export class ShopParams {
  // Vars for the selected filter items
  brandId = 0;
  typeId = 0;

  // Vars for sorting
  sort = 'name';

  // Pagination parameters
  pageNumber = 1;
  pageSize = 6;

  // Search params
  search!: string;
}
