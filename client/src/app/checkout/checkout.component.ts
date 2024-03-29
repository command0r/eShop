import {Component, OnInit} from '@angular/core';
import {UntypedFormBuilder, UntypedFormGroup, Validators} from "@angular/forms";
import {AccountService} from "../account/account.service";
import {Observable} from "rxjs";
import {IBasketTotals} from "../shared/models/basket";
import {BasketService} from "../basket/basket.service";

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss']
})
export class CheckoutComponent implements OnInit {
  basketTotal$!: Observable<IBasketTotals>;
  checkoutForm!: UntypedFormGroup;

  constructor(private fb: UntypedFormBuilder, private accountService: AccountService, private basketService: BasketService) {
  }

  ngOnInit(): void {
    this.createCheckoutForm();
    this.getAddressFromValues();
    this.getDeliveryMethodValue();
    this.basketTotal$ = this.basketService.basketTotals$;
  }

  createCheckoutForm() {
    this.checkoutForm = this.fb.group({
      addressForm: this.fb.group({
        firstName: [null, Validators.required],
        lastName: [null, Validators.required],
        street: [null, Validators.required],
        city: [null, Validators.required],
        state: [null, Validators.required],
        zipCode: [null, Validators.required]
      }),
      deliveryForm: this.fb.group({
        deliveryMethod: [null, Validators.required]
      }),
      paymentForm: this.fb.group({
        nameOnCard: [null, Validators.required]
      })
    });
  }

  getAddressFromValues() {
    this.accountService.getUserAddress().subscribe(address => {
      if (address) {
        this.checkoutForm.get('addressForm')?.patchValue(address);
      }
    }, error => {
      console.log(error);
    });
  }

  getDeliveryMethodValue()
  {
    const basket = this.basketService.getCurrentBasketValue();
    if(basket.deliveryMethodId !== null){
      // @ts-ignore
      this.checkoutForm.get('deliveryForm').get('deliveryMethod').patchValue(basket.deliveryMethodId.toString());
    }
  }
}
