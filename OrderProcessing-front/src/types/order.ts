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

export interface PagedList<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
