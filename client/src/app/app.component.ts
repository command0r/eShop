import {Component, OnInit} from '@angular/core';
import {BasketService} from "./basket/basket.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'E-Commerce';
  // TypeScript 2.7 and up has strict class checking and the property has to be initialized
  // or marked as 'not null' (by adding '!'). Note: "products: any[] = []" will work as well
  //products!: IProduct[];

  // Constructor
  constructor(private basketService: BasketService) { }

  // Lifecycle methods
  ngOnInit(): void {
    const basketId = localStorage.getItem('basket_id');
    if(basketId) {
      this.basketService.getBasket(basketId).subscribe(() => {
        console.log('Initialized basket');
      }, error => {
        console.log(error);
      })
    }
  }
}
