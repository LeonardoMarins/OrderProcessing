import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { createOrder, getOrder, getOrders } from '../client/orders';
import type { CreateOrderPayload } from '../types/order';

export function useOrders(page: number = 1, pageSize: number = 10) {
  return useQuery({
    queryKey: ['orders', page, pageSize],
    queryFn: () => getOrders(page, pageSize),
  });
}

export function useOrder(id: string) {
  return useQuery({
    queryKey: ['orders', id],
    queryFn: () => getOrder(id),
    staleTime: 0,
    enabled: !!id,
  });
}

export function useCreateOrder(onSuccess?: () => void) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (payload: CreateOrderPayload) => createOrder(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['orders'] });
      onSuccess?.();
    },
  });
}
