import { useState } from "react";

export default function Sidebar({ collapsed, setCollapsed }) {
  const [activeSection, setActiveSection] = useState("dashboard");
  const [lightTheme, setLightTheme] = useState(false);

  const toggleTheme = () => {
    setLightTheme(!lightTheme);
    document.body.classList.toggle("light");
  };

  const menuItems = [
    { id: "dashboard", icon: "bx-home", label: "Dashboard" },
    { id: "calendario", icon: "bx-calendar", label: "Calendário" },
    { id: "desempenho", icon: "bx-line-chart", label: "Desempenho" },
    { id: "materias", icon: "bx-book", label: "Matérias" },
    { id: "relatorio", icon: "bx-network-chart", label: "Relatório" },
  ];

  return (
    <aside className={`sidebar ${collapsed ? "collapsed" : ""}`}>
      <div className="toggle-btn" onClick={() => setCollapsed(!collapsed)}>
        <i className="bx bx-menu"></i>
      </div>

      <div className="logo">
        <h2>TIVIT Academy</h2>
      </div>

      <nav>
        <ul>
          {menuItems.map((item) => (
            <li
              key={item.id}
              className={activeSection === item.id ? "active" : ""}
              onClick={() => setActiveSection(item.id)}
            >
              <i className={`bx ${item.icon}`}></i> {item.label}
            </li>
          ))}
        </ul>
      </nav>

      <div className="theme-toggle">
        <input
          type="checkbox"
          checked={lightTheme}
          onChange={toggleTheme}
        />
      </div>
    </aside>
  );
}