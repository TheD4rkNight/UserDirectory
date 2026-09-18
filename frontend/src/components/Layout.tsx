import { Link, Outlet } from 'react-router-dom';
import { useIsAuthenticated, useMsal } from '@azure/msal-react';
import { loginRequest } from '../auth/authConfig';

export default function Layout() {
  const { instance } = useMsal();
  const authenticated = useIsAuthenticated();

  return (
    <div className="app-shell">
      <header className="topbar">
        <Link className="brand" to="/">User Directory</Link>
        <nav aria-label="Main navigation">
          <Link to="/">List</Link>
          <Link to="/add">Add</Link>
          {authenticated ? (
            <button onClick={() => instance.logoutRedirect()}>Sign out</button>
          ) : (
            <button onClick={() => instance.loginRedirect(loginRequest)}>Sign in</button>
          )}
        </nav>
      </header>
      <main className="container"><Outlet /></main>
    </div>
  );
}
