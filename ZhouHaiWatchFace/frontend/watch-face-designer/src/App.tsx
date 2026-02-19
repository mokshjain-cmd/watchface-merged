import { StoreProvider } from './providers/StoreProvider';
import { Layout } from './components/Layout/Layout';
import './App.scss';

function App() {
  return (
    <StoreProvider>
      <div className="App">
        <Layout />
      </div>
    </StoreProvider>
  );
}

export default App;
