import { useNavigate } from "react-router-dom";
import { useState } from "react";
import { login } from "../api/authApi";
import { saveToken } from "../auth/tokenStorage";

interface Props{
    onLogin: () => void;
}

export function Test() {
    return (
      <div className="bg-red-500 text-white p-10">
        TAILWIND TEST
      </div>
    );
  }

export function LoginPage( { onLogin }: Props){
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const navigate = useNavigate();

    const handleLogin = async()=>{
        try{
            const data = await login(email, password);
            saveToken(data.token);
            onLogin();
            navigate("/");
        }catch(error){
            if(error instanceof Error){
                alert("Login failed: " + error.message);
            }else{
                alert("Login failed.")
            }
            
        }
    }

    return(
        <div style={{ padding: 24}}>
            <h1>Login</h1>
            <input
                placeholder="Email"
                value={email}   
                onChange={(e) => setEmail(e.target.value)}
                style={{ display: "block", marginBottom: 12, width: "100%"}}/>
            <br/><br/>
            <input
                placeholder="Password"
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                style={{ display: "block", marginBottom: 12, width: "100%"}}/>
            <br/><br/>
            <button onClick={handleLogin}>Login</button>
            <br/><br/>
            <button onClick={()=>navigate("/register")}>Register</button>
        </div>
    )
}