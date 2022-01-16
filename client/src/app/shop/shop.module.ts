import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ShopComponent} from './shop.component';
import { ProductItemComponent } from './product-item/product-item.component';
import {SharedModule} from "../shared/shared.module";
import { ProductDetailsComponent } from './product-details/product-details.component';
import {ShopRoutingModule} from "./shop-routing.module";

@NgModule({
  declarations: [ShopComponent, ProductItemComponent, ProductDetailsComponent],
  imports: [
    CommonModule,
    SharedModule,
    // Import ShopRoutingModule instead of just RoutingModule (lazy-loading implementation)
    ShopRoutingModule
  ]
  // App module is no longer responsible for loading this component
  //exports: [ShopComponent]
})
export class ShopModule {
}
