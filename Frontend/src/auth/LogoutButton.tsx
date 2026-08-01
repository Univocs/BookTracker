import { useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { removeAccessToken } from "./tokenStorage";

export function LogoutButton() {
  const queryClient = useQueryClient();  // reaches into the cache
  const navigate = useNavigate();        // redirecting 

  function logout() {   
    removeAccessToken();                 // removes acces token
    queryClient.removeQueries({ queryKey: ["current-member"] });
    navigate("/login");
  }

  return <button onClick={logout}>Log out</button>;
}