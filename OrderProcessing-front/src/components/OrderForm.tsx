import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { useCreateOrder } from '../hooks/use-order';

interface FormValues {
  client: string;
  value: number;
  orderData: string;
}

export function OrderForm() {
  const [successId, setSuccessId] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<FormValues>({
    defaultValues: {
      client: '',
      value: undefined,
      orderData: new Date().toISOString().slice(0, 16),
    },
  });

  const mutation = useCreateOrder((id) => {
    setSuccessId(id);
    reset({ client: '', value: undefined, orderData: new Date().toISOString().slice(0, 16) });
  });

  const onSubmit = (data: FormValues) => {
    setSuccessId(null);
    mutation.mutate({
      id: crypto.randomUUID(),
      client: data.client.trim(),
      value: Number(data.value),
      orderData: new Date(data.orderData).toISOString(),
    });
  };

  return (
    <section className="card">
      <h2 className="card-title">Novo Pedido</h2>

      {successId && (
        <div className="alert alert-success">
          Pedido enviado com sucesso!<br />
          <span className="small">ID: {successId}</span>
        </div>
      )}

      {mutation.isError && (
        <div className="alert alert-error">
          {mutation.error instanceof Error ? mutation.error.message : 'Erro ao criar pedido'}
        </div>
      )}

      <form onSubmit={handleSubmit(onSubmit)} className="form">
        <div className="form-group">
          <label htmlFor="client">Cliente</label>
          <input
            id="client"
            type="text"
            placeholder="Nome do cliente"
            {...register('client', { required: 'O nome do cliente é obrigatório' })}
          />
          {errors.client && <span className="field-error">{errors.client.message}</span>}
        </div>

        <div className="form-group">
          <label htmlFor="value">Valor (R$)</label>
          <input
            id="value"
            type="number"
            placeholder="0,00"
            step="0.01"
            {...register('value', {
              required: 'O valor é obrigatório',
              min: { value: 0.01, message: 'O valor deve ser maior que zero' },
            })}
          />
          {errors.value && <span className="field-error">{errors.value.message}</span>}
        </div>

        <div className="form-group">
          <label htmlFor="orderData">Data do Pedido</label>
          <input
            id="orderData"
            type="datetime-local"
            {...register('orderData', { required: 'A data é obrigatória' })}
          />
          {errors.orderData && <span className="field-error">{errors.orderData.message}</span>}
        </div>

        <button type="submit" className="btn" disabled={mutation.isPending}>
          {mutation.isPending ? 'Enviando...' : 'Cadastrar Pedido'}
        </button>
      </form>
    </section>
  );
}
