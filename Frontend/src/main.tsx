import { StrictMode } from 'react' // Lets you find common bugs in your components during development.
import { createRoot } from 'react-dom/client' // lets you create a root to display React components.
import { BrowserRouter } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import App from "./App";
import "./index.css";

const queryClient = new QueryClient(); // houdt alle cache/config bij, zoals DbContect in backend.

createRoot(document.getElementById("root")!).render(
  <StrictMode>                                    // Gets all dev checks from above.
    <QueryClientProvider client={queryClient}>    // injects queryclient with useQuery.
      <BrowserRouter>                             // Routes app and children with URL that is active.
        <App />                                   // Here the rest of the UI gets build
      </BrowserRouter>
    </QueryClientProvider>
  </StrictMode>
);