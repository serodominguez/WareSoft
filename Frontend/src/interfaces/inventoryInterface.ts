export interface Inventory {
  idStore: number | null;
  idProduct: number | null;
  code: string;
  description: string;
  material: string;
  color: string;
  unitMeasure: string,
  stock: number | null;
  price: number | null;
  brandName: string;
  categoryName: string;
}