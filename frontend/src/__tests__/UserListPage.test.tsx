import { render, screen, waitFor } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import UserListPage from '../pages/UserListPage';
import { getUsers } from '../api/usersApi';

vi.mock('../api/usersApi', () => ({ getUsers: vi.fn() }));

const mockedGetUsers = vi.mocked(getUsers);

beforeEach(() => vi.resetAllMocks());

describe('UserListPage', () => {
  it('shows spinner while loading', () => {
    mockedGetUsers.mockReturnValue(new Promise(() => undefined));
    render(<UserListPage />);
    expect(screen.getByRole('status')).toBeInTheDocument();
  });

  it('renders users returned by the API', async () => {
    mockedGetUsers.mockResolvedValue([{ id: 1, name: 'Jane Doe', age: 40, city: 'Melbourne', state: 'VIC', pincode: '3000' }]);
    render(<UserListPage />);
    expect(await screen.findByText('Jane Doe')).toBeInTheDocument();
    expect(screen.getByText('Melbourne')).toBeInTheDocument();
    expect(screen.getByText('VIC')).toBeInTheDocument();
  });

  it('shows empty state', async () => {
    mockedGetUsers.mockResolvedValue([]);
    render(<UserListPage />);
    expect(await screen.findByText('No users found.')).toBeInTheDocument();
  });

  it('shows API error', async () => {
    mockedGetUsers.mockRejectedValue(new Error('API unavailable'));
    render(<UserListPage />);
    await waitFor(() => expect(screen.getByRole('alert')).toHaveTextContent('API unavailable'));
  });
});
