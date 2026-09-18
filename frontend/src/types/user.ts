export interface User {
  id: number;
  name: string;
  age: number;
  city: string;
  state: string;
  pincode: string;
}

export interface CreateUserRequest {
  name: string;
  age: number;
  city: string;
  state: string;
  pincode: string;
}

export type UpdateUserRequest = CreateUserRequest;
