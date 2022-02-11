import { Component, OnInit } from '@angular/core';
import {Observable} from "rxjs";
import {IBasket} from "../../shared/models/basket";
import {BasketService} from "../../basket/basket.service";
import {ToastrService} from "ngx-toastr";

@Component({
  selector: 'app-checkout-review',
  templateUrl: './checkout-review.component.html',
  styleUrls: ['./checkout-review.component.scss']
})
export class CheckoutReviewComponent implements OnInit {
  basket$!: Observable<IBasket>;

  constructor(private basketService: BasketService, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.basket$ = this.basketService.basket$;
  }

  createPaymentIntent() {
    return this.basketService.createPaymentIntent().subscribe((response: any) => {
      this.toastr.success('Payment intent created');
    }, error => {
      console.log(error);
      this.toastr.error(error.message);
    })
  }
}
