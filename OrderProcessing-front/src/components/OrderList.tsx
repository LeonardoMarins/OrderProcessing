import { useState } from 'react';
import { useOrders } from '../hooks/use-order';
import { OrderDetail } from './OrderDetail';

const formatCurrency = (value: number) =>
  new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);

const formatDate = (iso: string) =>
  new Intl.DateTimeFormat('pt-BR', { dateStyle: 'short', timeStyle: 'short' }).format(new Date(iso));

export function OrderList() {
  const [selectedId, setSelectedId] = useState<string | null>(null);

  const { data: orders, isLoading, isError, error, refetch } = useOrders();

  if (isLoading) {
    return (
      <section className="card">
        <h2 className="card-title">Pedidos</h2>
        <p className="text-muted">Carregando...</p>
      </section>
    );
  }

  if (isError) {
    return (
      <section className="card">
        <h2 className="card-title">Pedidos</h2>
        <div className="alert alert-error">
          {error instanceof Error ? error.message : 'Erro ao carregar pedidos'}
        </div>
        <button className="btn btn-outline" onClick={() => refetch()}>Tentar novamente</button>
      </section>
    );
  }

  return (
    <section className="card">
      <div className="card-header">
        <h2 className="card-title">Pedidos</h2>
        <button className="btn btn-outline btn-sm" onClick={() => refetch()}>Atualizar</button>
      </div>

      {selectedId && (
        <OrderDetail id={selectedId} onClose={() => setSelectedId(null)} />
      )}

      {!orders || orders.length === 0 ? (
        <p className="text-muted">Nenhum pedido encontrado.</p>
      ) : (
        <div className="table-wrapper">
          <table className="table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Cliente</th>
                <th>Valor</th>
                <th>Data</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {orders.map((order) => (
                <tr key={order.id}>
                  <td className="id-cell" title={order.id}>{order.id.slice(0, 8)}…</td>
                  <td>{order.client}</td>
                  <td>{formatCurrency(order.value)}</td>
                  <td>{formatDate(order.orderDate)}</td>
                  <td>
                    <button
                      className="btn btn-outline btn-sm"
                      onClick={() => setSelectedId(order.id)}
                    >
                      Ver
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  );
}
