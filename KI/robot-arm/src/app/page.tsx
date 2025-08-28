import Image from "next/image";
import styles from "./page.module.css";
import ThreeScene from "@/components/arm";

export default function Home() {
  return (
    <>
      <h1>Hello World</h1>
      <ThreeScene className="scene" />
    </>
  );
}
