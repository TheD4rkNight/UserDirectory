import { useEffect, useState } from 'react';
import { getUsers } from '../api/usersApi';
import Spinner from '../components/Spinner';
import type { User } from '../types/user';

export default function UserListPage() {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const controller = new AbortController();
    void getUsers(controller.signal)
      .then(setUsers)
      .catch((err: unknown) => {
        if (!controller.signal.aborted) {
          setError(err instanceof Error ? err.message : 'Unable to load users.');
        }
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false);
      });
    return () => controller.abort();
  }, []);

  if (loading) return <Spinner />;
  if (error) return <div className="error" role="alert"><h2>Unable to load users</h2><p>{error}</p></div>;

  return (
    <section>
      <div className="page-heading">
        <div>
          <h1>User List</h1>
          <p>All users currently stored in the directory.</p>
        </div>
      </div>

      {users.length === 0 ? (
        <div className="empty">No users found.</div>
      ) : (
        <div className="table-card">
          <table>
            <thead>
              <tr><th>Name</th><th>Age</th><th>City</th><th>State</th><th>Pincode</th></tr>
            </thead>
            <tbody>
              {users.map(user => (
                <tr key={user.id}>
                  <td>{user.name}</td><td>{user.age}</td><td>{user.city}</td><td>{user.state}</td><td>{user.pincode}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  );
}
