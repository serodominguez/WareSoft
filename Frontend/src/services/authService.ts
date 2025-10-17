import axios, { AxiosResponse } from 'axios';
import { jwtDecode } from 'jwt-decode';

export interface LoginCredentials {
  user: string;
  password: string;
}

export interface LoginResponse {
  token: string;
}

export interface DecodedToken {
  exp: number;
  [key: string]: any;
}

class AuthService {
  private readonly TOKEN_KEY = 'token';
  private readonly API_URL = 'api/Users/Generate';

  async login(credentials: LoginCredentials): Promise<LoginResponse> {
    const response: AxiosResponse<LoginResponse> = await axios.post(
      this.API_URL,
      credentials
    );
    return response.data;
  }

  saveToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  removeToken(): void {
    localStorage.removeItem(this.TOKEN_KEY);
  }

  decodeToken(token: string): DecodedToken | null {
    try {
      return jwtDecode<DecodedToken>(token);
    } catch (error) {
      console.error('Error al decodificar token:', error);
      return null;
    }
  }

  isTokenExpired(token: string): boolean {
    const decoded = this.decodeToken(token);
    if (!decoded) return true;

    const currentTime = Date.now() / 1000;
    return decoded.exp < currentTime;
  }

  validateToken(): boolean {
    const token = this.getToken();
    if (!token) return false;

    return !this.isTokenExpired(token);
  }

  getCurrentUserFromToken(): DecodedToken | null {
    const token = this.getToken();
    if (!token) return null;

    if (this.isTokenExpired(token)) {
      this.removeToken();
      return null;
    }

    return this.decodeToken(token);
  }
}

export default new AuthService();