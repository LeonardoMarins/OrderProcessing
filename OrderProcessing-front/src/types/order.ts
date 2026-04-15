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
  orderDate: string;
}

export interface PagedList<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
