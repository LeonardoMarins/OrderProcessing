export interface Order {
  id: string;
  client: string;
  value: number;
  orderDate: string;
}

export interface CreateOrderPayload {
  id: string;
  client: string;
  value: number;
  orderData: string;
}
