// User entity types

export interface User {
  id: string;
  googleId: string;
  email: string;
  name?: string;
  givenName?: string;
  familyName?: string;
  pictureUrl?: string;
  locale?: string;
  createdAt: string;
  updatedAt: string;
  lastLoginAt?: string;
}

// Current user DTO from /auth/me endpoint
export interface CurrentUser {
  id?: string;
  email?: string;
  name?: string;
  picture?: string;
}
