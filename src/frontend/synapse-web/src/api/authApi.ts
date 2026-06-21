const API_BASE = "http://localhost:8080/api/auth";

export interface AuthResponse {
    token: string;
    refreshToken?: string;
    success: boolean;
    code?: string;
    message?: string;
}

export async function login(email: string, password: string): Promise<AuthResponse>{
    const response = await fetch(`${API_BASE}/login`,{
        method: "POST",
        headers:{
            "Content-Type":"application/json",
        },
        body: JSON.stringify({
            email, 
            password,
        }),
    });
    if (!response.ok){
        throw new Error("Login failed");
    }

    return response.json();
}

export async function register(name: string, email: string, password: string): Promise<AuthResponse>{
    const response = await fetch(`${API_BASE}/register`,{
        method: "POST",
        headers:{
            "Content-Type":"application/json",
        },
        body: JSON.stringify({
            name,
            email, 
            password,
        }),
    });
    if (!response.ok){
        throw new Error("Register failed");
    }

    return response.json();
}

export async function refreshToken(refreshToken: string): Promise<AuthResponse>{
    const response = await fetch(`${API_BASE}/refresh`,{
        method: "POST",
        headers:{
            "Content-Type":"application/json",
        },
        body: JSON.stringify({
            refreshToken,
        }),
    });
    if (!response.ok){
        throw new Error("Token refresh failed");
    }

    return response.json();
}

export function getOAuthUrl(provider: "google" | "microsoft", returnUrl?: string): string {
    const baseUrl = "http://localhost:8080/api/auth/oauth";
    const redirectUrl = encodeURIComponent(returnUrl || "http://localhost:3000");
    return `${baseUrl}/${provider}?returnUrl=${redirectUrl}`;
}