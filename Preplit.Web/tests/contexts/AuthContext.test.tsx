import { render, screen, act } from '@testing-library/react';
import { AuthProvider, useAuth, reducer } from '../../src/pages/contexts/AuthContext';
import { jwtDecode } from 'jwt-decode';
import React from 'react';

jest.mock('jwt-decode');


describe('authReducer', () => {
  it('should handle LOGIN', () => {
    const action = { type: 'LOGIN', payload: { id: '123', email: 'test@example.com' } };
    const initialState = { user: null as any, isAuthenticated: false };
    const result = reducer(initialState, action);
    expect(result).toEqual({ user: action.payload, isAuthenticated: true });
  });

  it('should handle LOGOUT', () => {
    const initialState = { user: { id: '123' }, isAuthenticated: true };
    const result = reducer(initialState, { type: 'LOGOUT' });
    expect(result).toEqual({ user: null, isAuthenticated: false });
  });
});


describe('AuthProvider behavior', () => {
  beforeEach(() => {
    localStorage.clear();
    jest.resetAllMocks();
  });

  const MockConsumer = () => {
    const { user, isAuthenticated } = useAuth();
    return (
      <>
        <div data-testid="auth">{isAuthenticated ? 'Yes' : 'No'}</div>
        <div data-testid="user">{user ? user.fullName : 'None'}</div>
      </>
    );
  };

  it('loads user from valid token on mount', async () => {
    localStorage.setItem('jwt', 'fakeToken');
    (jwtDecode as jest.Mock).mockReturnValue({
      exp: Date.now(),
      "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress": "user@example.com",
      "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier": "123",
      "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "User",
      "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": "User Name",
    });

    await act(async () =>
      render(
        <AuthProvider>
          <MockConsumer />
        </AuthProvider>
      )
    );

    expect(screen.getByTestId('auth').textContent).toBe('Yes');
    expect(screen.getByTestId('user').textContent).toBe('User Name');
  });
});