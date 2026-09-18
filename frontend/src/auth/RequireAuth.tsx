import { useIsAuthenticated, useMsal } from '@azure/msal-react';
import { loginRequest } from './authConfig';

export default function RequireAuth({ children }: { children: React.ReactNode }) {
  const isAuthenticated = useIsAuthenticated();
  const { instance } = useMsal();
  if (!isAuthenticated) {
    return (
      <div className="auth-required">
        <h2>Sign in required</h2>
        <p>You need to sign in to add users.</p>
        <button onClick={() => instance.loginRedirect(loginRequest)}>Sign in</button>
      </div>
    );
  }

  return children;
}
