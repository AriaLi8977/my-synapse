//centralize auth
const TOKEN_KEY = "synapse_token"
const REFRESH_TOKEN_KEY = "synapse_refresh_token"

export function saveToken(token: string){
    localStorage.setItem(TOKEN_KEY, token);
}

export function getToken(){
    return localStorage.getItem(TOKEN_KEY);
}

export function clearToken(){
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
}

export function saveRefreshToken(token: string){
    localStorage.setItem(REFRESH_TOKEN_KEY, token);
}

export function getRefreshToken(){
    return localStorage.getItem(REFRESH_TOKEN_KEY);
}