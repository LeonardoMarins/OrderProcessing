import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { useCreateOrder } from '../hooks/use-order';

interface FormValues {
  client: string;
  value: number;
  orderDate: string;
}

export function OrderForm() {
  const [success, setSuccess] = useState(false);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<FormValues>({
    defaultValues: {
      client: '',
      value: undefined,
      orderDate: new Date().toISOString().slice(0, 16),
    },
  });

  const mutation = useCreateOrder(() => {
    setSuccess(true);
    reset({ client: '', value: undefined, orderDate: new Date().toISOString().slice(0, 16) });
  });

  const onSubmit = (data: FormValues) => {
    setSuccess(false);
    mutation.mutate({
      id: crypto.randomUUID(),
      client: data.client.trim(),
      value: Number(data.value),
      orderDate: new Date(data.orderDate).toISOString(),
    });
  };

  return (
    <section className="card">
      <h2 className="card-title">Novo Pedido</h2>

      {success && (
        <div className="alert alert-success">
          Pedido enviado com sucesso!
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
          <label htmlFor="orderDate">Data do Pedido</label>
          <input
            id="orderDate"
            type="datetime-local"
            {...register('orderDate', { required: 'A data do pedido é obrigatória' })}
          />
          {errors.orderDate && <span className="field-error">{errors.orderDate.message}</span>}
        </div>

        <button type="submit" className="btn" disabled={mutation.isPending}>
          {mutation.isPending ? 'Enviando...' : 'Cadastrar Pedido'}
        </button>
      </form>
    </section>
  );
}
