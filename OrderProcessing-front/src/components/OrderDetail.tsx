import { useOrder } from '../hooks/use-order';

interface Props {
  id: string;
  onClose: () => void;
}

const formatCurrency = (value: number) =>
  new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);

const formatDate = (iso: string) =>
  new Intl.DateTimeFormat('pt-BR', { dateStyle: 'long', timeStyle: 'short' }).format(new Date(iso));

export function OrderDetail({ id, onClose }: Props) {
  const { data: order, isLoading, isError, error } = useOrder(id);

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h3>Detalhes do Pedido</h3>
          <button className="modal-close" onClick={onClose} aria-label="Fechar">✕</button>
        </div>

        <div className="modal-body">
          {isLoading && <p className="text-muted">Carregando...</p>}

          {isError && (
            <div className="alert alert-error">
              {error instanceof Error ? error.message : 'Erro ao carregar pedido'}
            </div>
          )}

          {order && (
            <dl className="detail-list">
              <div className="detail-row">
                <dt>ID</dt>
                <dd className="mono">{order.id}</dd>
              </div>
              <div className="detail-row">
                <dt>Cliente</dt>
                <dd>{order.client}</dd>
              </div>
              <div className="detail-row">
                <dt>Valor</dt>
                <dd>{formatCurrency(order.value)}</dd>
              </div>
              <div className="detail-row">
                <dt>Data do Pedido</dt>
                <dd>{formatDate(order.orderDate)}</dd>
              </div>
            </dl>
          )}
        </div>
      </div>
    </div>
  );
}
