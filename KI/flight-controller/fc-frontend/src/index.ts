import './index.css';

// Types matching the backend data structures
interface Airplane {
    callsign: string;
    aircraft_type: string;
    latitude: number;
    longitude: number;
    altitude: number;
    speed: number;
    heading: number;
}

interface Alert {
    plane1_callsign: string;
    plane2_callsign: string;
    distance_nm: number;
    altitude_diff_ft: number;
}

interface EventData {
    planes: Airplane[];
    alerts: Alert[];
}

class RadarApp {
    private canvas: HTMLCanvasElement;
    private ctx: CanvasRenderingContext2D;
    private aircraftDetailsElement: HTMLElement;
    private alertsListElement: HTMLElement;
    private eventSource: EventSource | null = null;
    private planes: Airplane[] = [];
    private alerts: Alert[] = [];
    private selectedAircraft: string | null = null;
    private aircraftElements: Map<string, HTMLElement> = new Map();
    
    // Radar configuration
    private readonly RADAR_RADIUS = 380; // Canvas is 800x800, so radius is ~380
    private readonly CENTER_X = 400;
    private readonly CENTER_Y = 400;
    private currentRangeKm = 100; // Current zoom level in kilometers
    private readonly ZOOM_LEVELS = [20, 40, 60, 80, 100]; // Available zoom levels
    
    // Linz Airport coordinates (center of radar)
    private readonly CENTER_LAT = 48.238575;
    private readonly CENTER_LNG = 14.191473;

    constructor() {
        this.canvas = document.getElementById('radar-canvas') as HTMLCanvasElement;
        this.ctx = this.canvas.getContext('2d')!;
        this.aircraftDetailsElement = document.getElementById('aircraft-details')!;
        this.alertsListElement = document.getElementById('alerts-list')!;
        
        this.setupEventListeners();
        this.connectToBackend();
        this.updateZoomControls();
        this.startRadarAnimation();
    }

    private setupEventListeners(): void {
        // Close aircraft details popup
        const closeBtn = document.getElementById('close-details')!;
        closeBtn.addEventListener('click', () => {
            this.hideAircraftDetails();
        });

        // Canvas click handler for selecting aircraft
        this.canvas.addEventListener('click', (event) => {
            const rect = this.canvas.getBoundingClientRect();
            const x = event.clientX - rect.left;
            const y = event.clientY - rect.top;
            
            // Find clicked aircraft
            const clickedAircraft = this.findAircraftAtPosition(x, y);
            if (clickedAircraft) {
                this.selectAircraft(clickedAircraft);
            } else {
                this.hideAircraftDetails();
            }
        });

        // Hide popup when clicking outside
        document.addEventListener('click', (event) => {
            if (!this.aircraftDetailsElement.contains(event.target as Node) && 
                event.target !== this.canvas) {
                this.hideAircraftDetails();
            }
        });

        // Zoom controls
        const zoomInBtn = document.getElementById('zoom-in')!;
        const zoomOutBtn = document.getElementById('zoom-out')!;
        
        zoomInBtn.addEventListener('click', () => {
            this.zoomIn();
        });
        
        zoomOutBtn.addEventListener('click', () => {
            this.zoomOut();
        });
    }

    private connectToBackend(): void {
        // Connect to the SSE endpoint
        this.eventSource = new EventSource('http://127.0.0.1:3000/sse');
        
        this.eventSource.onmessage = (event) => {
            try {
                const data: EventData = JSON.parse(event.data);
                this.planes = data.planes;
                this.alerts = data.alerts;
                this.updateDisplay();
            } catch (error) {
                console.error('Error parsing SSE data:', error);
            }
        };

        this.eventSource.onerror = (error) => {
            console.error('SSE connection error:', error);
            // Attempt to reconnect after 5 seconds
            setTimeout(() => {
                this.connectToBackend();
            }, 5000);
        };
    }

    private startRadarAnimation(): void {
        this.drawRadar();
        requestAnimationFrame(() => this.startRadarAnimation());
    }

    private drawRadar(): void {
        // Clear canvas
        this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
        
        // Draw radar circles and grid
        this.drawRadarGrid();
        
        // Draw aircraft
        this.drawAircraft();
        
        // Draw range rings labels
        this.drawRangeLabels();
    }

    private drawRadarGrid(): void {
        this.ctx.strokeStyle = '#00ff00';
        this.ctx.lineWidth = 1;
        
        // Draw concentric circles (range rings)
        // Calculate range rings based on current zoom level
        const ranges = this.calculateRangeRings();
        ranges.forEach(range => {
            const radius = (range / this.currentRangeKm) * this.RADAR_RADIUS;
            this.ctx.beginPath();
            this.ctx.arc(this.CENTER_X, this.CENTER_Y, radius, 0, 2 * Math.PI);
            this.ctx.setLineDash([5, 5]);
            this.ctx.stroke();
        });
        
        // Draw crosshairs
        this.ctx.setLineDash([]);
        this.ctx.beginPath();
        // Horizontal line
        this.ctx.moveTo(this.CENTER_X - this.RADAR_RADIUS, this.CENTER_Y);
        this.ctx.lineTo(this.CENTER_X + this.RADAR_RADIUS, this.CENTER_Y);
        // Vertical line
        this.ctx.moveTo(this.CENTER_X, this.CENTER_Y - this.RADAR_RADIUS);
        this.ctx.lineTo(this.CENTER_X, this.CENTER_Y + this.RADAR_RADIUS);
        this.ctx.stroke();
    }

    private drawRangeLabels(): void {
        this.ctx.fillStyle = '#00ff00';
        this.ctx.font = '12px Courier New';
        this.ctx.textAlign = 'center';
        
        const ranges = this.calculateRangeRings();
        ranges.forEach(range => {
            const radius = (range / this.currentRangeKm) * this.RADAR_RADIUS;
            this.ctx.fillText(`${range}km`, this.CENTER_X + radius - 20, this.CENTER_Y + 15);
        });
    }

    private drawAircraft(): void {
        this.planes.forEach(plane => {
            const position = this.latLngToRadarPosition(plane.latitude, plane.longitude);
            if (position) {
                const isAlert = this.isAircraftInAlert(plane.callsign);
                const isSelected = this.selectedAircraft === plane.callsign;
                
                // Draw aircraft dot
                this.ctx.beginPath();
                this.ctx.arc(position.x, position.y, isSelected ? 6 : 4, 0, 2 * Math.PI);
                this.ctx.fillStyle = isAlert ? '#ff0000' : isSelected ? '#ffaa00' : '#00ff00';
                this.ctx.fill();
                
                // Add glow effect
                this.ctx.shadowColor = this.ctx.fillStyle;
                this.ctx.shadowBlur = isSelected ? 15 : 10;
                this.ctx.fill();
                this.ctx.shadowBlur = 0;
                
                // Draw callsign and flight level
                this.ctx.fillStyle = isAlert ? '#ff0000' : '#00ff00';
                this.ctx.font = '10px Courier New';
                this.ctx.textAlign = 'left';
                
                const flightLevel = Math.round(plane.altitude / 100);
                this.ctx.fillText(plane.callsign, position.x + 8, position.y - 2);
                this.ctx.fillText(`FL${flightLevel}`, position.x + 8, position.y + 10);
            }
        });
    }

    private latLngToRadarPosition(lat: number, lng: number): { x: number; y: number } | null {
        // Calculate distance from center (Linz Airport)
        const distance = this.haversineDistance(this.CENTER_LAT, this.CENTER_LNG, lat, lng);
        
        // Check if within radar range
        if (distance > this.currentRangeKm) {
            return null;
        }
        
        // Calculate bearing from center
        const bearing = this.calculateBearing(this.CENTER_LAT, this.CENTER_LNG, lat, lng);
        
        // Convert to radar coordinates
        const radarDistance = (distance / this.currentRangeKm) * this.RADAR_RADIUS;
        const x = this.CENTER_X + radarDistance * Math.sin(bearing);
        const y = this.CENTER_Y - radarDistance * Math.cos(bearing);
        
        return { x, y };
    }

    private haversineDistance(lat1: number, lng1: number, lat2: number, lng2: number): number {
        const R = 6371; // Earth's radius in kilometers
        const dLat = (lat2 - lat1) * Math.PI / 180;
        const dLng = (lng2 - lng1) * Math.PI / 180;
        const a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
                  Math.cos(lat1 * Math.PI / 180) * Math.cos(lat2 * Math.PI / 180) *
                  Math.sin(dLng / 2) * Math.sin(dLng / 2);
        const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
        return R * c;
    }

    private calculateBearing(lat1: number, lng1: number, lat2: number, lng2: number): number {
        const dLng = (lng2 - lng1) * Math.PI / 180;
        const lat1Rad = lat1 * Math.PI / 180;
        const lat2Rad = lat2 * Math.PI / 180;
        
        const y = Math.sin(dLng) * Math.cos(lat2Rad);
        const x = Math.cos(lat1Rad) * Math.sin(lat2Rad) -
                  Math.sin(lat1Rad) * Math.cos(lat2Rad) * Math.cos(dLng);
        
        return Math.atan2(y, x);
    }

    private findAircraftAtPosition(x: number, y: number): Airplane | null {
        const clickRadius = 15; // Pixels
        
        for (const plane of this.planes) {
            const position = this.latLngToRadarPosition(plane.latitude, plane.longitude);
            if (position) {
                const distance = Math.sqrt(
                    Math.pow(x - position.x, 2) + Math.pow(y - position.y, 2)
                );
                if (distance <= clickRadius) {
                    return plane;
                }
            }
        }
        return null;
    }

    private selectAircraft(aircraft: Airplane): void {
        this.selectedAircraft = aircraft.callsign;
        this.showAircraftDetails(aircraft);
    }

    private showAircraftDetails(aircraft: Airplane): void {
        // Update aircraft details popup
        document.getElementById('aircraft-callsign')!.textContent = aircraft.callsign;
        document.getElementById('aircraft-model')!.textContent = aircraft.aircraft_type;
        document.getElementById('aircraft-speed')!.textContent = `${Math.round(aircraft.speed)} KT`;
        document.getElementById('aircraft-heading')!.textContent = `${Math.round(aircraft.heading)}°`;
        document.getElementById('aircraft-altitude')!.textContent = `${Math.round(aircraft.altitude)} FT`;
        document.getElementById('aircraft-position')!.innerHTML = 
            `${aircraft.latitude.toFixed(4)}°N<br/>${aircraft.longitude.toFixed(4)}°E`;
        
        // Position popup near the aircraft
        const position = this.latLngToRadarPosition(aircraft.latitude, aircraft.longitude);
        if (position) {
            const rect = this.canvas.getBoundingClientRect();
            this.aircraftDetailsElement.style.left = `${rect.left + position.x + 20}px`;
            this.aircraftDetailsElement.style.top = `${rect.top + position.y - 50}px`;
        }
        
        this.aircraftDetailsElement.classList.add('visible');
    }

    private hideAircraftDetails(): void {
        this.selectedAircraft = null;
        this.aircraftDetailsElement.classList.remove('visible');
    }

    private isAircraftInAlert(callsign: string): boolean {
        return this.alerts.some(alert => 
            alert.plane1_callsign === callsign || alert.plane2_callsign === callsign
        );
    }

    private updateDisplay(): void {
        this.updateAlertsPanel();
    }

    private updateAlertsPanel(): void {
        this.alertsListElement.innerHTML = '';
        
        if (this.alerts.length === 0) {
            const noAlerts = document.createElement('div');
            noAlerts.textContent = 'No alerts';
            noAlerts.style.color = '#00ff00';
            noAlerts.style.textAlign = 'center';
            noAlerts.style.fontStyle = 'italic';
            this.alertsListElement.appendChild(noAlerts);
            return;
        }
        
        this.alerts.forEach(alert => {
            const alertElement = document.createElement('div');
            alertElement.className = 'alert-item';
            alertElement.innerHTML = `
                <strong>PROXIMITY ALERT</strong><br/>
                ${alert.plane1_callsign} ↔ ${alert.plane2_callsign}<br/>
                Distance: ${alert.distance_nm.toFixed(2)} NM<br/>
                Alt Diff: ${Math.round(alert.altitude_diff_ft)} FT
            `;
            this.alertsListElement.appendChild(alertElement);
        });
    }

    private calculateRangeRings(): number[] {
        // Generate range rings based on current zoom level
        const step = this.currentRangeKm / 5; // Always show 5 rings
        const ranges: number[] = [];
        for (let i = 1; i <= 4; i++) {
            ranges.push(step * i);
        }
        return ranges;
    }

    private zoomIn(): void {
        const currentIndex = this.ZOOM_LEVELS.indexOf(this.currentRangeKm);
        if (currentIndex > 0) {
            this.currentRangeKm = this.ZOOM_LEVELS[currentIndex - 1];
            this.updateZoomControls();
        }
    }

    private zoomOut(): void {
        const currentIndex = this.ZOOM_LEVELS.indexOf(this.currentRangeKm);
        if (currentIndex < this.ZOOM_LEVELS.length - 1) {
            this.currentRangeKm = this.ZOOM_LEVELS[currentIndex + 1];
            this.updateZoomControls();
        }
    }

    private updateZoomControls(): void {
        const zoomInBtn = document.getElementById('zoom-in') as HTMLButtonElement;
        const zoomOutBtn = document.getElementById('zoom-out') as HTMLButtonElement;
        const zoomLevelDisplay = document.getElementById('zoom-level')!;
        
        // Update zoom level display
        zoomLevelDisplay.textContent = `${this.currentRangeKm}km`;
        
        // Update button states
        const currentIndex = this.ZOOM_LEVELS.indexOf(this.currentRangeKm);
        zoomInBtn.disabled = currentIndex === 0; // Can't zoom in more (20km is max zoom)
        zoomOutBtn.disabled = currentIndex === this.ZOOM_LEVELS.length - 1; // Can't zoom out more (100km)
    }
}

// Initialize the application when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    new RadarApp();
});