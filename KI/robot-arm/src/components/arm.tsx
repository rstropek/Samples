'use client';

import { useEffect, useRef, useState } from 'react';
import * as THREE from 'three';

interface ThreeSceneProps {
  className?: string;
}

export default function ThreeScene({ className = '' }: ThreeSceneProps) {
  const mountRef = useRef<HTMLDivElement>(null);
  const frameRef = useRef<number>(0);
  const [isClient, setIsClient] = useState(false);

  useEffect(() => {
    setIsClient(true);
  }, []);

  useEffect(() => {
    if (!isClient || !mountRef.current) return;

    const container = mountRef.current;
    const { clientWidth: width, clientHeight: height } = container;

    // Scene setup
    const scene = new THREE.Scene();
    scene.background = new THREE.Color(0x1a1a1a);

    // Camera setup
    const camera = new THREE.PerspectiveCamera(75, width / height, 0.1, 1000);
    camera.position.z = 5;

    // Renderer setup
    const renderer = new THREE.WebGLRenderer({ 
      antialias: true,
      alpha: true 
    });
    renderer.setSize(width, height);
    renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));

    // Add renderer to DOM
    container.appendChild(renderer.domElement);

    // Create a simple cube
    const geometry = new THREE.BoxGeometry();
    const material = new THREE.MeshStandardMaterial({ 
      color: 0x00ff88,
      metalness: 0.7,
      roughness: 0.3
    });
    const cube = new THREE.Mesh(geometry, material);
    scene.add(cube);

    // Add lighting
    const ambientLight = new THREE.AmbientLight(0x404040, 0.4);
    scene.add(ambientLight);

    const directionalLight = new THREE.DirectionalLight(0xffffff, 1);
    directionalLight.position.set(5, 5, 5);
    scene.add(directionalLight);

    // Animation loop
    const animate = () => {
      frameRef.current = requestAnimationFrame(animate);
      
      // Rotate the cube
      cube.rotation.x += 0.01;
      cube.rotation.y += 0.01;
      
      renderer.render(scene, camera);
    };

    animate();

    // Cleanup function
    return () => {
      if (frameRef.current) {
        cancelAnimationFrame(frameRef.current);
      }
      
      if (container && renderer.domElement) {
        container.removeChild(renderer.domElement);
      }
      
      // Dispose of Three.js resources
      geometry.dispose();
      material.dispose();
      renderer.dispose();
    };
  }, [isClient]);

  if (!isClient) {
    return (
      <div>
        <span>Loading 3D Scene...</span>
      </div>
    );
  }

  return <div ref={mountRef} className={className} />;
}