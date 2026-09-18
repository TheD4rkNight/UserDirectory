import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter } from 'react-router-dom';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import AddUserPage from '../pages/AddUserPage';
import { createUser } from '../api/usersApi';

const navigate = vi.fn();
const acquireTokenSilent = vi.fn();

vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual<typeof import('react-router-dom')>('react-router-dom');
  return { ...actual, useNavigate: () => navigate };
});

vi.mock('@azure/msal-react', () => ({
  useIsAuthenticated: () => true,
  useMsal: () => ({ instance: { acquireTokenSilent }, accounts: [{ username: 'test@example.com' }] }),
}));

vi.mock('../api/usersApi', () => ({ createUser: vi.fn() }));

const mockedCreateUser = vi.mocked(createUser);

beforeEach(() => {
  vi.resetAllMocks();
  acquireTokenSilent.mockResolvedValue({ accessToken: 'test-token' });
});

describe('AddUserPage', () => {
  it('shows inline validation for required fields', async () => {
    const user = userEvent.setup();
    render(<MemoryRouter><AddUserPage /></MemoryRouter>);
    await user.click(screen.getByRole('button', { name: 'Add User' }));
    expect(screen.getByText('Name is required.')).toBeInTheDocument();
    expect(screen.getByText('Age is required.')).toBeInTheDocument();
    expect(screen.getByText('City is required.')).toBeInTheDocument();
    expect(screen.getByText('State is required.')).toBeInTheDocument();
    expect(screen.getByText('Pincode is required.')).toBeInTheDocument();
  });

  it('rejects an invalid age', async () => {
    const user = userEvent.setup();
    render(<MemoryRouter><AddUserPage /></MemoryRouter>);
    await user.type(screen.getByLabelText('Name'), 'Jane Doe');
    await user.type(screen.getByLabelText('Age'), '121');
    await user.type(screen.getByLabelText('City'), 'Melbourne');
    await user.type(screen.getByLabelText('State'), 'VIC');
    await user.type(screen.getByLabelText('Pincode'), '3000');
    await user.click(screen.getByRole('button', { name: 'Add User' }));
    expect(screen.getByText('Age must be an integer between 0 and 120.')).toBeInTheDocument();
    expect(mockedCreateUser).not.toHaveBeenCalled();
  });

  it('submits valid data and redirects', async () => {
    const user = userEvent.setup();
    mockedCreateUser.mockResolvedValue({ id: 1, name: 'Jane Doe', age: 40, city: 'Melbourne', state: 'VIC', pincode: '3000' });
    render(<MemoryRouter><AddUserPage /></MemoryRouter>);
    await user.type(screen.getByLabelText('Name'), 'Jane Doe');
    await user.type(screen.getByLabelText('Age'), '40');
    await user.type(screen.getByLabelText('City'), 'Melbourne');
    await user.type(screen.getByLabelText('State'), 'VIC');
    await user.type(screen.getByLabelText('Pincode'), '3000');
    await user.click(screen.getByRole('button', { name: 'Add User' }));
    await waitFor(() => expect(mockedCreateUser).toHaveBeenCalledWith(
      { name: 'Jane Doe', age: 40, city: 'Melbourne', state: 'VIC', pincode: '3000' },
      'test-token',
    ));
    expect(screen.getByRole('status')).toHaveTextContent('User created successfully.');
  });
});
