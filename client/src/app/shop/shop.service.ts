import { Injectable } from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import {IPagination, Pagination} from "../shared/models/pagination";
import {IBrand} from "../shared/models/brand";
import {IType} from "../shared/models/productType";
import {delay, map} from 'rxjs/operators';
import {ShopParams} from "../shared/models/shopParams";
import {IProduct} from "../shared/models/product";
import {of} from "rxjs";
import {environment} from "../../environments/environment";

@Injectable({
  providedIn: 'root'
})

export class ShopService {

  // Provide a base URL
  baseURL = environment.apiUrl;

  products: IProduct[] = [];
  brands: IBrand[] = [];
  types: IType[] = [];
  pagination = new Pagination();
  shopParams = new ShopParams();
  productCache = new Map();

  // Injecting the HttpClient here, instead of the components
  // (because components are to consume the services, not to instantiate)
  constructor(private http: HttpClient) { }

  // Method to get Products (and return paginated model with <>)
  getProducts(useCache: boolean) {
    if(!useCache) {
      this.productCache = new Map();
    }

    if(this.productCache.size > 0 && useCache) {
      // check if we have a key that mathes the shop params, so the value can be taken from a cache
      if(this.productCache.has(Object.values(this.shopParams).join('-'))) {
        this.pagination.data = this.productCache.get(Object.values(this.shopParams).join('-'));
        // return observable of pagination (if there's a matching key in the cache)
        return of(this.pagination)
      }
    }

    // Create 'params' object to pass as part of the query string
    let params = new HttpParams();

    if(this.shopParams.brandId !== 0) {
      params = params.append('brandId', this.shopParams.brandId.toString());
    }

    if(this.shopParams.typeId !== 0) {
      params = params.append('typeId', this.shopParams.typeId.toString());
    }

    // Search params
    if(this.shopParams.search) {
      params = params.append('search', this.shopParams.search);
    }

    // Sorting
    params = params.append('sort', this.shopParams.sort);

    // Pagination
    params = params.append('pageIndex', this.shopParams.pageNumber.toString());
    params = params.append('pageSize', this.shopParams.pageSize.toString());

    // 'pipe()' is a wrapper around 'rxjs' operators in case the syntax is used
    return this.http.get<IPagination>(this.baseURL + 'products', {observe: 'response', params})
      .pipe(
        // delay(1000),
        map(response => {
          // returning result back to a component
          // return response.body;

          // return products to an array (instead of response body)
          // this.products = response.body!.data;

          // use spread operator (...) to append new set of results from the API to the existing set of results
          // this.products = [...this.products, ...response.body!.data];

          this.productCache.set(Object.values(this.shopParams).join('-'), response.body?.data);

          // @ts-ignore
          this.pagination = response.body;
          return this.pagination;
          // return response.body;
        })
      )
  }

  // Set and get ShopParams
  setShopParams(params: ShopParams) {
    this.shopParams = params;
  }

  getShopParams() {
    return this.shopParams;
  }

  // Get product by ID
  getProduct(id: number) {
    let product: IProduct;
    this.productCache.forEach((products: IProduct[]) => {
      // @ts-ignore
      product = products.find(p => p.id === id);
    })

    // Check if we have products in an array (part of caching)
    // const product = this.products.find(p => p.id === id);
    // @ts-ignore
    if (product) {
      // returning an observable 'of' smth.
      // it allows going straight to a product on the shop page (instead of loading a product from an API)
      return of(product);
    }

    return this.http.get<IProduct>(this.baseURL + 'products/' + id)
  }

  // Get brands
  getBrands() {
    // get results from the array instead of the API (caching)
    if(this.brands.length > 0) {
      return of(this.brands);
    }

    // this is a default expression for getting data straight from the API every time we request data
    // return this.http.get<IBrand[]>(this.baseURL + 'products/brands');

    return this.http.get<IBrand[]>(this.baseURL + 'products/brands').pipe(
      map(reponse => {
        this.brands = reponse;
        return reponse;
      })
    );
  }

  // Get product types
  getTypes() {
    if(this.types.length > 0) {
      return of(this.types);
    }

    // return this.http.get<IType[]>(this.baseURL + 'products/types');

    // populate types from an array and map response back
    // i.e., we're storing data on the client side and return it from the array the next time
    return this.http.get<IType[]>(this.baseURL + 'products/types').pipe(
      map(reponse => {
        this.types = reponse;
        return reponse;
      })
    );
  }
}
