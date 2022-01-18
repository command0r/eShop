import { Component, OnInit } from '@angular/core';
import {IProduct} from "../../shared/models/product";
import {ShopService} from "../shop.service";
import {ActivatedRoute} from "@angular/router";
import {BreadcrumbModule, BreadcrumbService} from "xng-breadcrumb";

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss']
})
export class ProductDetailsComponent implements OnInit {
  product!: IProduct;

  // activatedRouter gives access to the route parameters, so the Product Id can be read from the URL
  constructor(private shopService: ShopService, private activatedRoute: ActivatedRoute, private bcService: BreadcrumbService)
  { }

  ngOnInit(): void {
    this.loadProduct();
  }

  // Load product by ID
  loadProduct() {
    // @ts-ignore
    this.shopService.getProduct(+this.activatedRoute.snapshot.paramMap.get('id')).subscribe(product => {
      this.product = product;
      // Access the routing alias to configure a breadcrumb (so it displays the product name instead of Id)
      this.bcService.set('@productDetails', product.name);
    }, error => {
      console.log(error);
    });
  }
}
