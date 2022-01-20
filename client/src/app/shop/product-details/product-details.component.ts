import { Component, OnInit } from '@angular/core';
import {IProduct} from "../../shared/models/product";
import {ShopService} from "../shop.service";
import {ActivatedRoute} from "@angular/router";
import {BreadcrumbModule, BreadcrumbService} from "xng-breadcrumb";
import {BasketService} from "../../basket/basket.service";

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss']
})
export class ProductDetailsComponent implements OnInit {
  product!: IProduct;
  quantity = 1;

  // activatedRouter gives access to the route parameters, so the Product Id can be read from the URL
  constructor(private shopService: ShopService, private activatedRoute: ActivatedRoute,
              private bcService: BreadcrumbService, private basketService: BasketService) {
    // As the product loads in the breadcrumb, the string will be empty. It'll be populated in 'loadProduct()' method
    // This is to avoid showing a Product ID while loading, instead of Product Name
    this.bcService.set('@productDetails', ' ');
  }

  ngOnInit(): void {
    this.loadProduct();
  }

  addItemToBasket() {
    this.basketService.addItemToBasket(this.product, this.quantity);
  }

  incrementQuantity() {
    this.quantity++;
  }

  decrementQuantity(){
    if(this.quantity > 1){
      this.quantity--;
    }
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
