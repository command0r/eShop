import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {IProduct} from "../shared/models/product";
import {ShopService} from "./shop.service";
import {IBrand} from "../shared/models/brand";
import {IType} from "../shared/models/productType";
import {ShopParams} from "../shared/models/shopParams";

@Component({
  selector: 'app-shop',
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.scss']
})
export class ShopComponent implements OnInit {

  // Search property. The property need to be not 'static' as it's controlled by the '*ngIf' on the page
  @ViewChild('search', {static: false}) searchTerm!: ElementRef;

  // Use created service
  products!: IProduct[];
  brands!: IBrand[];
  types!: IType[];
  shopParams!: ShopParams;
  totalCount!: number;

  sortOptions = [
    {name: 'Alphabetical', value: 'name'},
    {name: 'Price: Low to High', value: 'priceAsc'},
    {name: 'Price: High to Low', value: 'priceDesc'}
  ];

  // Inject shop service
  constructor(private shopService: ShopService) {
    this.shopParams = this.shopService.getShopParams();
  }

  ngOnInit() {
    // Call methods (so they're available)
    this.getProducts(true);
    this.getBrands();
    this.getTypes();
  }

  getProducts(useCache = false) {
    // Gets access to a backend API
    // we need to 'subscribe' to make use of an 'observable'
    this.shopService.getProducts(useCache)
      .subscribe(response => {
        this.products = response!.data;
        // not gonna set these since this was moved to a servie
        // this.shopParams.pageNumber = response!.pageIndex;
        // this.shopParams.pageSize = response!.pageSize;
        this.totalCount = response!.count;

    }, error => {
      console.log(error);
    })
  }

  getBrands() {
    this.shopService.getBrands().subscribe(response => {
      // Propagate 'all' option to the top of the list
      this.brands = [{id: 0, name: 'All'}, ...response];
    }, error => {
      console.log(error);
    });
  }

  getTypes() {
    this.shopService.getTypes().subscribe(response => {
      this.types =  [{id: 0, name: 'All'}, ...response];
    }, error => {
      console.log(error);
    });
  }

  onBrandSelected(brandId: number) {
    // this is to set the value inside the service (instead of doing it locally)
    const params = this.shopService.getShopParams();

    params.brandId = brandId;
    params.pageNumber = 1;

    // this way the service is going to 'remember' what we're passing in when we get products
    this.shopService.setShopParams(params);

    this.getProducts();
  }

  onTypeSelected(typeId: number) {
    const params = this.shopService.getShopParams();
    params.typeId = typeId;
    params.pageNumber = 1;
    this.shopService.setShopParams(params);
    this.getProducts();
  }

  onSortSelected(sort: string) {
    const params = this.shopService.getShopParams();
    params.sort = sort;
    this.shopService.setShopParams(params);
    this.getProducts();
  }

  // Page changed event handling (for pagination)
  onPageChanged(event: any) {
    const params = this.shopService.getShopParams();
    if(params.pageNumber !== event)
    {
      params.pageNumber = event;
      this.shopService.setShopParams(params);
      this.getProducts(true );
    }
  }

  // Search method
  onSearch() {
    const params = this.shopService.getShopParams();
    params.search = this.searchTerm.nativeElement.value;
    params.pageNumber = 1;
    this.shopService.setShopParams(params);
    this.getProducts();
  }

  onReset() {
    this.searchTerm.nativeElement.value = '';
    this.shopParams = new ShopParams();
    this.shopService.setShopParams(this.shopParams);
    this.getProducts();
  }
}
