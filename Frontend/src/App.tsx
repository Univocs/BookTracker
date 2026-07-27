import { Link, Route, Routes } from "react-router-dom";

// Link   = klikbare navigatielink, geen page reload
// Route  = koppelt 1 URL-pad aan 1 component
// Routes = kiest welke Route past bij de huidige URL

function HomePage() {                // Component
  return <h1>Book Tracker</h1>;
}

function AboutPage() {               // Component
  return <h1>About Book Tracker</h1>;
}

export default function App() {
  return (
    <>
      <nav>
        <Link to="/">Home</Link>{" "}
        <Link to="/about">About</Link>
      </nav>

      <Routes> 
        <Route path="/" element={<HomePage />} />
        <Route path="/about" element={<AboutPage />} />
      </Routes>
    </>
  );
}