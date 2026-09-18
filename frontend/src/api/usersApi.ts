import type { CreateUserRequest, User } from '../types/user';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? '/api';

async function parseError(response: Response): Promise<string> {
  const contentType = response.headers.get('content-type') ?? '';
  if (contentType.includes('application/json')) {
    const body = (await response.json()) as { detail?: string; title?: string };
    return body.detail ?? body.title ?? `Request failed (${response.status})`;
  }
  const text = await response.text();
  return text || `Request failed (${response.status})`;
}

async function request<T>(input: RequestInfo | URL, init?: RequestInit): Promise<T> {
  const response = await fetch(input, init);
  if (!response.ok) throw new Error(await parseError(response));
  return response.json() as Promise<T>;
}

export function getUsers(signal?: AbortSignal): Promise<User[]> {
  return request<User[]>(`${API_BASE_URL}/users`, { signal });
}

export function createUser(user: CreateUserRequest, accessToken: string): Promise<User> {
  return request<User>(`${API_BASE_URL}/users`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${accessToken}`,
    },
    body: JSON.stringify(user),
  });
}
