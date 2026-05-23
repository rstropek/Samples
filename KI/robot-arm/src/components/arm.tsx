'use client';

import { useEffect, useRef, useState } from 'react';
import * as THREE from 'three';
import { OrbitControls } from 'three-stdlib';
import { JointValues } from './slider';
import { calculateVerticalPipetteTilt } from '../lib/pipette-utils';

// Constants for robot arm dimensions (in meters)
const BASE_RADIUS = 0.25; // 50cm diameter = 25cm radius
const BASE_HEIGHT = 0.20; // 20cm height

const SEGMENT1_WIDTH = 0.10; // 10cm
const SEGMENT1_HEIGHT = 0.10; // 10cm
const SEGMENT1_LENGTH = 0.60; // 60cm

const SEGMENT2_WIDTH = 0.08; // 8cm
const SEGMENT2_HEIGHT = 0.08; // 8cm
const SEGMENT2_LENGTH = 0.45; // 45cm

const SEGMENT3_WIDTH = 0.06; // 6cm
const SEGMENT3_HEIGHT = 0.06; // 6cm
const SEGMENT3_LENGTH = 0.25; // 25cm

const PIPETTE_RADIUS = 0.01; // 2cm diameter = 1cm radius
const PIPETTE_HEIGHT = 0.10; // 10cm height

// Joint sphere radii
const SHOULDER_JOINT_RADIUS = 0.06; // 6cm
const ELBOW_JOINT_RADIUS = 0.05; // 5cm
const WRIST_JOINT_RADIUS = 0.04; // 4cm
const PIPETTE_JOINT_RADIUS = 0.015; // 1.5cm

// Default home pose angles (in degrees, will convert to radians)
const DEFAULT_HOME_POSE = {
  j0: 0,      // Base yaw around world Y
  j1: 75,     // Shoulder pitch around local X (negative to point upward)
  j2: 45,     // Elbow pitch around local X (negative to bend upward)
  j3: 15,     // Wrist pitch around local X (negative to keep upward orientation)
  j4: 0       // Pipette tilt calculated automatically for vertical orientation
};

interface ThreeSceneProps {
  className?: string;
  jointValues?: JointValues;
}

export default function ThreeScene({ className = '', jointValues }: ThreeSceneProps) {
  const mountRef = useRef<HTMLDivElement>(null);
  const frameRef = useRef<number>(0);
  const [isClient, setIsClient] = useState(false);
  
  // Refs to store joint groups for animation
  const j0JointRef = useRef<THREE.Group>(null);
  const j1JointRef = useRef<THREE.Group>(null);
  const j2JointRef = useRef<THREE.Group>(null);
  const j3JointRef = useRef<THREE.Group>(null);
  const j4JointRef = useRef<THREE.Group>(null);
  const rendererRef = useRef<THREE.WebGLRenderer>(null);
  const sceneRef = useRef<THREE.Scene>(null);
  const cameraRef = useRef<THREE.PerspectiveCamera>(null);
  const controlsRef = useRef<OrbitControls>(null);

  useEffect(() => {
    setIsClient(true);
  }, []);

  // Update joint rotations when jointValues change
  useEffect(() => {
    if (!jointValues || !j0JointRef.current || !j1JointRef.current || 
        !j2JointRef.current || !j3JointRef.current || !j4JointRef.current) {
      return;
    }

    // Calculate the required J4 angle to keep pipette vertical
    const calculatedJ4 = calculateVerticalPipetteTilt(jointValues.j1, jointValues.j2, jointValues.j3);

    // Update joint rotations based on slider values
    j0JointRef.current.rotation.y = THREE.MathUtils.degToRad(jointValues.j0);
    j1JointRef.current.rotation.x = THREE.MathUtils.degToRad(-jointValues.j1); // Negative for correct direction
    j2JointRef.current.rotation.x = THREE.MathUtils.degToRad(jointValues.j2);
    j3JointRef.current.rotation.x = THREE.MathUtils.degToRad(jointValues.j3);
    j4JointRef.current.rotation.x = THREE.MathUtils.degToRad(calculatedJ4);

    // Note: No manual re-render needed as animation loop handles continuous rendering
  }, [jointValues]);

  useEffect(() => {
    if (!isClient || !mountRef.current) return;

    const container = mountRef.current;
    const { clientWidth: width, clientHeight: height } = container;

    // Scene setup
    const scene = new THREE.Scene();
    scene.background = new THREE.Color(0xf0f0f0);
    sceneRef.current = scene;

    // Camera setup
    const camera = new THREE.PerspectiveCamera(60, width / height, 0.1, 1000);
    camera.position.set(1.6, 1.1, 1.8);
    camera.lookAt(0, 0, 0);
    cameraRef.current = camera;

    // Renderer setup with shadows
    const renderer = new THREE.WebGLRenderer({ 
      antialias: true,
      alpha: true 
    });
    renderer.setSize(width, height);
    renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));
    renderer.shadowMap.enabled = true;
    renderer.shadowMap.type = THREE.PCFSoftShadowMap;
    rendererRef.current = renderer;

    // Add renderer to DOM
    container.appendChild(renderer.domElement);

    // Setup OrbitControls for mouse camera movement
    const controls = new OrbitControls(camera, renderer.domElement);
    controls.enableDamping = true; // Enable smooth camera movement
    controls.dampingFactor = 0.05;
    controls.screenSpacePanning = false;
    controls.minDistance = 0.5; // Minimum zoom distance
    controls.maxDistance = 10; // Maximum zoom distance
    controls.maxPolarAngle = Math.PI; // Allow full vertical rotation
    controls.target.set(0, 0.5, 0); // Look at a point slightly above the base
    controlsRef.current = controls;

    // Use provided joint values or defaults
    const currentValues = jointValues || DEFAULT_HOME_POSE;
    
    // Calculate the initial J4 value for vertical pipette
    const initialJ4 = calculateVerticalPipetteTilt(currentValues.j1, currentValues.j2, currentValues.j3);

    // Materials
    const baseMaterial = new THREE.MeshStandardMaterial({ 
      color: 0x404040, 
      metalness: 0.3, 
      roughness: 0.7 
    });
    const segment1Material = new THREE.MeshStandardMaterial({ 
      color: 0x606060, 
      metalness: 0.3, 
      roughness: 0.7 
    });
    const segment2Material = new THREE.MeshStandardMaterial({ 
      color: 0x707070, 
      metalness: 0.3, 
      roughness: 0.7 
    });
    const segment3Material = new THREE.MeshStandardMaterial({ 
      color: 0x808080, 
      metalness: 0.3, 
      roughness: 0.7 
    });
    const pipetteMaterial = new THREE.MeshStandardMaterial({ 
      color: 0xffffff, 
      metalness: 0.1, 
      roughness: 0.5 
    });
    const jointMaterial = new THREE.MeshStandardMaterial({ 
      color: 0x303030, 
      metalness: 0.5, 
      roughness: 0.6 
    });

    // Create geometries
    const baseGeometry = new THREE.CylinderGeometry(BASE_RADIUS, BASE_RADIUS, BASE_HEIGHT, 32);
    const segment1Geometry = new THREE.BoxGeometry(SEGMENT1_WIDTH, SEGMENT1_HEIGHT, SEGMENT1_LENGTH);
    const segment2Geometry = new THREE.BoxGeometry(SEGMENT2_WIDTH, SEGMENT2_HEIGHT, SEGMENT2_LENGTH);
    const segment3Geometry = new THREE.BoxGeometry(SEGMENT3_WIDTH, SEGMENT3_HEIGHT, SEGMENT3_LENGTH);
    const pipetteGeometry = new THREE.CylinderGeometry(PIPETTE_RADIUS, PIPETTE_RADIUS, PIPETTE_HEIGHT, 16);
    
    const shoulderJointGeometry = new THREE.SphereGeometry(SHOULDER_JOINT_RADIUS, 16, 16);
    const elbowJointGeometry = new THREE.SphereGeometry(ELBOW_JOINT_RADIUS, 16, 16);
    const wristJointGeometry = new THREE.SphereGeometry(WRIST_JOINT_RADIUS, 16, 16);
    const pipetteJointGeometry = new THREE.SphereGeometry(PIPETTE_JOINT_RADIUS, 16, 16);

    // Create robot arm hierarchy
    const robotArm = new THREE.Group();
    scene.add(robotArm);

    // Base (positioned so bottom rests on ground)
    const baseMesh = new THREE.Mesh(baseGeometry, baseMaterial);
    baseMesh.position.y = BASE_HEIGHT / 2;
    baseMesh.castShadow = true;
    baseMesh.receiveShadow = true;
    robotArm.add(baseMesh);

    // J0 (base yaw joint) - rotates around world Y
    const j0Joint = new THREE.Group();
    j0Joint.position.y = BASE_HEIGHT;
    j0Joint.rotation.y = THREE.MathUtils.degToRad(currentValues.j0);
    robotArm.add(j0Joint);
    j0JointRef.current = j0Joint;

    // Shoulder joint sphere
    const shoulderJointMesh = new THREE.Mesh(shoulderJointGeometry, jointMaterial);
    shoulderJointMesh.castShadow = true;
    j0Joint.add(shoulderJointMesh);

    // J1 (shoulder pitch joint) - rotates around local X
    const j1Joint = new THREE.Group();
    j1Joint.rotation.x = THREE.MathUtils.degToRad(-currentValues.j1); // Negative for correct direction
    j0Joint.add(j1Joint);
    j1JointRef.current = j1Joint;

    // Segment 1 (shoulder link) - extends along +Z
    const segment1Mesh = new THREE.Mesh(segment1Geometry, segment1Material);
    segment1Mesh.position.z = SEGMENT1_LENGTH / 2;
    segment1Mesh.castShadow = true;
    segment1Mesh.receiveShadow = true;
    j1Joint.add(segment1Mesh);

    // Elbow joint sphere
    const elbowJointMesh = new THREE.Mesh(elbowJointGeometry, jointMaterial);
    elbowJointMesh.position.z = SEGMENT1_LENGTH;
    elbowJointMesh.castShadow = true;
    j1Joint.add(elbowJointMesh);

    // J2 (elbow pitch joint) - rotates around local X
    const j2Joint = new THREE.Group();
    j2Joint.position.z = SEGMENT1_LENGTH;
    j2Joint.rotation.x = THREE.MathUtils.degToRad(currentValues.j2);
    j1Joint.add(j2Joint);
    j2JointRef.current = j2Joint;

    // Segment 2 (elbow link) - extends along +Z
    const segment2Mesh = new THREE.Mesh(segment2Geometry, segment2Material);
    segment2Mesh.position.z = SEGMENT2_LENGTH / 2;
    segment2Mesh.castShadow = true;
    segment2Mesh.receiveShadow = true;
    j2Joint.add(segment2Mesh);

    // Wrist joint sphere
    const wristJointMesh = new THREE.Mesh(wristJointGeometry, jointMaterial);
    wristJointMesh.position.z = SEGMENT2_LENGTH;
    wristJointMesh.castShadow = true;
    j2Joint.add(wristJointMesh);

    // J3 (wrist pitch joint) - rotates around local X
    const j3Joint = new THREE.Group();
    j3Joint.position.z = SEGMENT2_LENGTH;
    j3Joint.rotation.x = THREE.MathUtils.degToRad(currentValues.j3);
    j2Joint.add(j3Joint);
    j3JointRef.current = j3Joint;

    // Segment 3 (wrist link) - extends along +Z
    const segment3Mesh = new THREE.Mesh(segment3Geometry, segment3Material);
    segment3Mesh.position.z = SEGMENT3_LENGTH / 2;
    segment3Mesh.castShadow = true;
    segment3Mesh.receiveShadow = true;
    j3Joint.add(segment3Mesh);

    // Pipette joint sphere
    const pipetteJointMesh = new THREE.Mesh(pipetteJointGeometry, jointMaterial);
    pipetteJointMesh.position.z = SEGMENT3_LENGTH;
    pipetteJointMesh.castShadow = true;
    j3Joint.add(pipetteJointMesh);

    // J4 (pipette tilt joint) - rotates around local X (calculated for vertical orientation)
    const j4Joint = new THREE.Group();
    j4Joint.position.z = SEGMENT3_LENGTH;
    j4Joint.rotation.x = THREE.MathUtils.degToRad(initialJ4);
    j3Joint.add(j4Joint);
    j4JointRef.current = j4Joint;

    // Pipette - extends along +Y (down)
    const pipetteMesh = new THREE.Mesh(pipetteGeometry, pipetteMaterial);
    pipetteMesh.position.y = -PIPETTE_HEIGHT / 2;
    pipetteMesh.castShadow = true;
    j4Joint.add(pipetteMesh);

    // Ground grid
    const gridHelper = new THREE.GridHelper(4, 40, 0x888888, 0xcccccc);
    scene.add(gridHelper);

    // Axes helper at world origin
    const axesHelper = new THREE.AxesHelper(0.2);
    scene.add(axesHelper);

    // Lighting
    const hemisphereLight = new THREE.HemisphereLight(0xffffff, 0x444444, 0.6);
    scene.add(hemisphereLight);

    const directionalLight = new THREE.DirectionalLight(0xffffff, 0.8);
    directionalLight.position.set(2, 3, 2);
    directionalLight.castShadow = true;
    directionalLight.shadow.mapSize.width = 2048;
    directionalLight.shadow.mapSize.height = 2048;
    directionalLight.shadow.camera.near = 0.5;
    directionalLight.shadow.camera.far = 10;
    directionalLight.shadow.camera.left = -3;
    directionalLight.shadow.camera.right = 3;
    directionalLight.shadow.camera.top = 3;
    directionalLight.shadow.camera.bottom = -3;
    scene.add(directionalLight);

    // Initial render
    renderer.render(scene, camera);

    // Animation loop for smooth controls
    const animate = () => {
      frameRef.current = requestAnimationFrame(animate);
      
      if (controls) {
        controls.update(); // Update controls for damping
      }
      
      renderer.render(scene, camera);
    };
    
    // Start animation loop
    animate();

    // Cleanup function
    return () => {
      if (frameRef.current) {
        cancelAnimationFrame(frameRef.current);
      }
      
      if (controls) {
        controls.dispose();
      }
      
      if (container && renderer.domElement) {
        container.removeChild(renderer.domElement);
      }
      
      // Dispose of Three.js resources
      baseGeometry.dispose();
      segment1Geometry.dispose();
      segment2Geometry.dispose();
      segment3Geometry.dispose();
      pipetteGeometry.dispose();
      shoulderJointGeometry.dispose();
      elbowJointGeometry.dispose();
      wristJointGeometry.dispose();
      pipetteJointGeometry.dispose();
      
      baseMaterial.dispose();
      segment1Material.dispose();
      segment2Material.dispose();
      segment3Material.dispose();
      pipetteMaterial.dispose();
      jointMaterial.dispose();
      
      renderer.dispose();
    };
  }, [isClient]);

  if (!isClient) {
    return (
      <div>
        <span>Loading 3D Robot Arm...</span>
      </div>
    );
  }

  return <div ref={mountRef} className={className} />;
}