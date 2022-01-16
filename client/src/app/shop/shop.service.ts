import { Injectable } from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import {IPagination} from "../shared/models/pagination";
import {IBrand} from "../shared/models/brand";
import {IType} from "../shared/models/productType";
import {delay, map} from 'rxjs/operators';
import {ShopParams} from "../shared/models/shopParams";
import {IProduct} from "../shared/models/product";

@Injectable({
  providedIn: 'root'
})

export class ShopService {

  // Provide a base URL
  baseURL = 'https://localhost:5001/api/';

  // Injecting the HttpClient here, instead of the components
  // (because components are to consume the services, not to instantiate)
  constructor(private http: HttpClient) { }

  // Method to get Products (and return paginated model with <>)
  getProducts(shopParams: ShopParams) {
    // Create 'params' object to pass as part of the query string
    let params = new HttpParams();

    if(shopParams.brandId !== 0) {
      params = params.append('brandId', shopParams.brandId.toString());
    }

    if(shopParams.typeId !== 0) {
      params = params.append('typeId', shopParams.typeId.toString());
    }

    // Search params
    if(shopParams.search) {
      params = params.append('search', shopParams.search);
    }

    // Sorting
    params = params.append('sort', shopParams.sort);

    // Pagination
    params = params.append('pageIndex', shopParams.pageNumber.toString());
    params = params.append('pageSize', shopParams.pageSize.toString());

    // 'pipe()' is a wrapper around 'rxjs' operators in case the syntax is used
    return this.http.get<IPagination>(this.baseURL + 'products', {observe: 'response', params})
      .pipe(
        //delay(1000),
        map(({body}) => {
          return body;
        })
      )
  }

  // Get product by ID
  getProduct(id: number) {
    return this.http.get<IProduct>(this.baseURL + 'products/' + id)
  }

  // Get brands
  getBrands() {
    return this.http.get<IBrand[]>(this.baseURL + 'products/brands');
  }

  // Get product types
  getTypes() {
    return this.http.get<IType[]>(this.baseURL + 'products/types');
  }
}
