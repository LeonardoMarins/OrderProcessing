export interface Order {
  id: string;
  client: string;
  value: number;
  orderDate: string;
}

export interface CreateOrderPayload {
  client: string;
  value: number;
}
