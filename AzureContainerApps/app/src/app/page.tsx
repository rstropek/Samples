"use client";

import { useState } from "react";
import styles from "./page.module.css";

export default function Home() {
  const [num1, setNum1] = useState<string>("");
  const [num2, setNum2] = useState<string>("");
  const [result, setResult] = useState<number | null>(null);

  const handleAdd = () => {
    const number1 = parseFloat(num1);
    const number2 = parseFloat(num2);
    
    if (!isNaN(number1) && !isNaN(number2)) {
      setResult(number1 + number2);
    } else {
      setResult(null);
    }
  };

  return (
    <div className={styles.page}>
      <main className={styles.main}>
        <div className={styles.calculator}>
          <h1>Simple Calculator</h1>
          <div className={styles.inputGroup}>
            <input
              type="number"
              placeholder="Enter first number"
              value={num1}
              onChange={(e) => setNum1(e.target.value)}
              className={styles.numberInput}
            />
            <input
              type="number"
              placeholder="Enter second number"
              value={num2}
              onChange={(e) => setNum2(e.target.value)}
              className={styles.numberInput}
            />
            <button onClick={handleAdd} className={styles.addButton}>
              Add
            </button>
          </div>
          {result !== null && (
            <div className={styles.result}>
              Result: {result}
            </div>
          )}
        </div>
      </main>
      <footer className={styles.footer}>
        Next Demo App
      </footer>
    </div>
  );
}
