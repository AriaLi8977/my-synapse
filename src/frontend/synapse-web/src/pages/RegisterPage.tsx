import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { register } from "../api/authApi";
import { saveToken } from "../auth/tokenStorage";

interface Props{
    onRegister: () => void;
}
export function RegisterPage( { onRegister }: Props){

const navigate = useNavigate();
    const [email,setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [name, setUserName] = useState("");

    const handleRegister = async () => {
        try{
            const data = await register(name, email, password);
            saveToken(data.token);
            onRegister();
            navigate("/");
        }catch(error){
            if(error instanceof Error){
                alert("Registration failed: " + error.message);
            }else{
                alert("Registration failed.");
            }
            
        }
    };
    return(
        <div style={{ padding: 24}}>
            <h1>Register</h1>
            <input
                placeholder="UserName"
                value={name}
                onChange={(e) => setUserName(e.target.value)}></input>
            <br/><br/>
            <input
                placeholder="Email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}></input>
            <br/><br/>
            <input
                placeholder="password"
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}></input>
            <br/><br/>
            <button onClick={handleRegister}>Register</button>
            <br/><br/>
            <button onClick={()=>navigate("/login")}>Back to Login</button>
        </div>
    )
}