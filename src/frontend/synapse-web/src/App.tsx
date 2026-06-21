import { Routes, Route, Navigate, useSearchParams } from "react-router-dom";
import { HomePage } from "./pages/HomePage";
import { LoginPage} from "./pages/LoginPage";
import { RegisterPage } from "./pages/RegisterPage";
import { NoteDetailPage } from "./pages/NoteDetailPage";
import { getToken, saveToken, clearToken } from "./auth/tokenStorage";
import { useState, useEffect } from "react";

function OAuthCallbackHandler({ onAuthChange }: { onAuthChange: () => void }) {
    const [searchParams] = useSearchParams();

    useEffect(() => {
        const token = searchParams.get("token");

        if (token) {
            clearToken(); // Clear any existing tokens
            saveToken(token);

            // Remove token from URL
            const cleanUrl = window.location.pathname;
            window.history.replaceState({}, document.title, cleanUrl);

            // Trigger auth state change
            onAuthChange();
        }
    }, [searchParams, onAuthChange]);

    return null;
}

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(!!getToken());
  const [, setForceUpdate] = useState(0);

  const handleAuthChange = () => {
      setIsAuthenticated(true);
      setForceUpdate(prev => prev + 1);
  };

  if (!isAuthenticated) {
    return(
      <>
        <OAuthCallbackHandler onAuthChange={handleAuthChange} />
        <Routes>
          <Route path="/login" element={<LoginPage onLogin={()=> setIsAuthenticated(true)}/>} />
          <Route path="/register" element={<RegisterPage onRegister={()=> setIsAuthenticated(true)}/>} />
          <Route path="*" element={<Navigate to="/login" />} />
        </Routes>
      </>
    )
  }

  return (
    <>
      <OAuthCallbackHandler onAuthChange={handleAuthChange} />
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/notes/:id" element={<NoteDetailPage />} />
      </Routes>
    </>
  );
}

export default App;