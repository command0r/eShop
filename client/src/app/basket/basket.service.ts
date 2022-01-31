import {Injectable} from '@angular/core';
import {environment} from "../../environments/environment";
import {HttpClient} from "@angular/common/http";
import {BehaviorSubject} from "rxjs";
import {Basket, IBasket, IBasketItem, IBasketTotals} from "../shared/models/basket";
import {map} from "rxjs/operators";
import {IProduct} from "../shared/models/product";
import {IDeliveryMethod} from "../shared/models/deliveryMethod";

@Injectable({
  providedIn: 'root'
})
export class BasketService {
  baseUrl = environment.apiUrl;
  // Once this is a BehaviorSubject, it'll always emit the initial values
  // @ts-ignore
  private basketSource = new BehaviorSubject<IBasket>(null);
  // @ts-ignore
  private basketTotalSource = new BehaviorSubject<IBasketTotals>(null);
  basket$ = this.basketSource.asObservable();
  basketTotals$ = this.basketTotalSource.asObservable();
  shipping = 0;

  constructor(private http: HttpClient) {
  }

  // Set shipping price
  setShippingPrice(deliveryMethod: IDeliveryMethod) {
    this.shipping = deliveryMethod.price;
    this.CalculateTotals();
  }

  // Get basket method
  getBasket(id: string) {
    return this.http.get(this.baseUrl + 'basket?id=' + id)
      // We need to set our basketSource with the basket we get from the API
      .pipe(
        // @ts-ignore
        map((basket: IBasket) => {
          this.basketSource.next(basket);
          this.CalculateTotals();
        })
      );
  }

  // Set basket method
  setBasket(basket: IBasket) {
    // This time we subscribe because we need to execute whatever we set to basket
    // @ts-ignore
    return this.http.post(this.baseUrl + 'basket', basket).subscribe((response: IBasket) => {
      this.basketSource.next(response);
      this.CalculateTotals();
    }, error => {
      console.log(error);
    })
  }

  // Helper method to get current basket values
  getCurrentBasketValue() {
    return this.basketSource.value;
  }

  // Adding stuff to basket
  addItemToBasket(item: IProduct, quantity = 1) {
    const itemToAdd: IBasketItem = this.mapProductItemToBasketItem(item, quantity);
    const basket = this.getCurrentBasketValue() ?? this.createBasket();
    basket.items = this.addOrUpdateItem(basket.items, itemToAdd, quantity);
    this.setBasket(basket);
  }

  // Making the 'increment/decrement' buttons work
  incrementItemQuantity(item: IBasketItem) {
    const basket = this.getCurrentBasketValue();
    const foundItemIndex = basket.items.findIndex(x => x.id === item.id);
    basket.items[foundItemIndex].quantity++;
    this.setBasket(basket);
  }

  decrementItemQuantity(item: IBasketItem) {
    const basket = this.getCurrentBasketValue();
    const foundItemIndex = basket.items.findIndex(x => x.id === item.id);
    if(basket.items[foundItemIndex].quantity > 1) {
      basket.items[foundItemIndex].quantity--;
      this.setBasket(basket);
    } else {
      this.removeItemFromBasket(item);
    }
  }

  // Method to remove items from basket (or the basket entirely)
  public removeItemFromBasket(item: IBasketItem) {
    const basket = this.getCurrentBasketValue();
    if(basket.items.some(x => x.id === item.id)) {
      basket.items = basket.items.filter(i => i.id !== item.id)
      if(basket.items.length > 0) {
        this.setBasket(basket);
      } else {
        this.deleteBasket(basket);
      }
    }
  }

  deleteLocalBasket(id: string) {
    this.basketSource.next(null!);
    this.basketTotalSource.next(null!);
    localStorage.removeItem('basket_id');
  }

  // Method to delete basket
  deleteBasket(basket: IBasket) {
    return this.http.delete(this.baseUrl + 'basket?id=' + basket.id).subscribe(() => {
      this.basketSource.next(null!);
      this.basketTotalSource.next(null!);
      localStorage.removeItem('basket_id');
    }, error => {
      console.log(error);
    });
  }

  // We need a method to map IProduct to IBasketItem
  private mapProductItemToBasketItem(item: IProduct, quantity: number) {
    return {
      id: item.id,
      productName: item.name,
      price: item.price,
      pictureUrl: item.pictureUrl,
      quantity,
      brand: item.productBrand,
      type: item.productType
    }
  }

  // Creating a new basket
  private createBasket(): IBasket {
    const basket = new Basket();
    // Local storage is used to store the basket Id temporarily
    localStorage.setItem('basket_id', basket.id);
    return basket;
  }

  // Method to check if the item is already available in the basket before updating
  private addOrUpdateItem(items: IBasketItem[], itemToAdd: IBasketItem, quantity: number): IBasketItem[] {
    console.log(items);
    const index = items.findIndex(i => i.id === itemToAdd.id);
    if (index === -1) {
      itemToAdd.quantity = quantity;
      items.push(itemToAdd);
    } else {
      items[index].quantity += quantity;
    }

    return items;
  }

  // Calculate basket totals
  private CalculateTotals() {
    const basket = this.getCurrentBasketValue();
    const shipping = this.shipping;
    // 'a' represents a number that's returned from the 'reduce' function. 'b' represents an item (with price and qty)
    const subtotal = basket.items.reduce((a, b) => (b.price * b.quantity) + a, 0);
    const total = subtotal + shipping;
    this.basketTotalSource.next({shipping, total, subtotal});
  }
}
