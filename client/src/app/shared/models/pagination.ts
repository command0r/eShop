import {IProduct} from "./product";

export interface IPagination {
  pageIndex: number;
  pageSize: number;
  count: number;
  data: IProduct[];
}

// logic to make pagination elements available in service (instead of just component)
export  class Pagination implements IPagination {
  pageIndex!: number;
  pageSize!: number;
  count!: number;
  data: IProduct[] = [];
}
