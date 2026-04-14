import type { CreateOrderPayload, Order, PagedList } from '../types/order';

const BASE_URL = import.meta.env.VITE_API_URL ?? '';

async function handleResponse<T>(res: Response): Promise<T> {
  if (!res.ok) {
    const error = await res.text();
    throw new Error(error || `HTTP ${res.status}`);
  }
  if (res.status === 204 || res.headers.get('content-length') === '0') {
    return undefined as T;
  }
  return res.json();
}

export const createOrder = async (payload: CreateOrderPayload): Promise<void> => {
  const res = await fetch(`${BASE_URL}/orders`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  });
  await handleResponse<void>(res);
};

export const getOrder = async (id: string): Promise<Order> => {
  const res = await fetch(`${BASE_URL}/orders/${id}`);
  return handleResponse<Order>(res);
};

export const getOrders = async (page: number, pageSize: number): Promise<PagedList<Order>> => {
  const res = await fetch(`${BASE_URL}/orders?page=${page}&pageSize=${pageSize}`);
  return handleResponse<PagedList<Order>>(res);
};
