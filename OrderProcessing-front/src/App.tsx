import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { OrderForm } from './components/OrderForm';
import { OrderList } from './components/OrderList';
import './App.css';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1,
      staleTime: 30_000,
    },
  },
});

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <div className="layout">
        <header className="header">
          <h1>Order Processing</h1>
          <span className="header-subtitle">Sistema de Gestão de Pedidos</span>
        </header>

        <main className="main">
          <OrderForm />
          <OrderList />
        </main>
      </div>

      <ReactQueryDevtools initialIsOpen={false} />
    </QueryClientProvider>
  );
}

export default App;
