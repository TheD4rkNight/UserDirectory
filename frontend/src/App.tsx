import { BrowserRouter, Route, Routes } from 'react-router-dom';
import Layout from './components/Layout';
import AddUserPage from './pages/AddUserPage';
import UserListPage from './pages/UserListPage';

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route element={<Layout />}>
          <Route path="/" element={<UserListPage />} />
          <Route path="/add" element={<AddUserPage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}
