import "./styles.css";

// Each step declares:
//   states:  css classes applied to the stage (order-independent)
//   caption: presenter-facing caption text (may include html for highlights)
const steps = [
  {
    states: [],
    caption: `<span class="mark-ink">A warehouse, a model.</span> An everyday dataset question.`,
  },
  {
    states: ["approach-1"],
    caption: `First instinct — <span class="mark-red">Approach 01</span> — hand the file to the model.`,
  },
  {
    states: ["approach-1", "s-prompt"],
    caption: `The question forms. &ldquo;Analyze this dataset.&rdquo;`,
  },
  {
    states: ["approach-1", "s-prompt", "s-csv-docked"],
    caption: `Attach the CSV. Two&nbsp;point&nbsp;three gigabytes of <em>everything</em>.`,
  },
  {
    states: ["approach-1", "s-prompt", "s-csv-docked", "s-flying-bad"],
    caption: `Ship it over the wire to the frontier model…`,
  },
  {
    states: ["approach-1", "s-prompt", "s-csv-docked", "s-flying-bad", "s-error"],
    caption: `…and everything that can go wrong, does. <span class="mark-red">Context · Privacy</span>`,
  },
  {
    states: ["approach-2"],
    caption: `Rewind. <span class="mark-green">Approach 02</span> — ship the code, not the data.`,
  },
  {
    states: ["approach-2", "s-prompt"],
    caption: `Same question. Different strategy.`,
  },
  {
    states: ["approach-2", "s-prompt", "s-metadata"],
    caption: `Peel off a <em>metadata</em> sketch — schema, dtypes, pseudonymised PII. Small. Safe.`,
  },
  {
    states: ["approach-2", "s-prompt-scripted", "s-prompt", "s-metadata", "s-metadata-docked"],
    caption: `Reframe the ask: <span class="mark-green">write a script</span>, don&rsquo;t read the data.`,
  },
  {
    states: ["approach-2", "s-prompt-scripted", "s-prompt", "s-metadata", "s-metadata-docked", "s-flying-good", "s-cloud-ok"],
    caption: `The model responds with Python — not conclusions. Intent, not content.`,
  },
  {
    states: ["approach-2", "s-cloud-ok", "s-script-emit", "s-sandbox"],
    caption: `A disposable sandbox spins up. The code and the full CSV meet <em>here</em>.`,
  },
  {
    states: ["approach-2", "s-cloud-ok", "s-sandbox", "s-ingest", "s-compute"],
    caption: `Execution happens in the box. Raw rows never leave it.`,
  },
  {
    states: ["approach-2", "s-cloud-ok", "s-sandbox", "s-ingest", "s-compute", "s-out"],
    caption: `Only the <span class="mark-green">answer</span> comes back.`,
  },

  /* ========== SCENARIO 2 — generating data ========== */
  {
    states: ["scenario-2"],
    caption: `<span class="mark-ink">A second puzzle.</span> The data doesn&rsquo;t exist yet — we need to generate it.`,
  },
  {
    states: ["scenario-2", "approach-1"],
    caption: `Instinct again — <span class="mark-red">Approach 01</span> — ask the model to just produce it.`,
  },
  {
    states: ["scenario-2", "approach-1", "s-prompt", "s-gen-prompt"],
    caption: `&ldquo;Generate a large dataset.&rdquo;`,
  },
  {
    states: ["scenario-2", "approach-1", "s-prompt", "s-gen-prompt", "s-prompt-flying"],
    caption: `Off it goes, with nothing to attach.`,
  },
  {
    states: ["scenario-2", "approach-1", "s-prompt", "s-gen-prompt", "s-prompt-flying", "s-llm-emit-csv"],
    caption: `The model begins streaming rows back as tokens…`,
  },
  {
    states: ["scenario-2", "approach-1", "s-prompt", "s-gen-prompt", "s-prompt-flying", "s-llm-emit-csv", "s-llm-csv-bad"],
    caption: `…and runs into the wall. <span class="mark-red">Context window exceeded</span>`,
  },
  {
    states: ["scenario-2", "approach-2"],
    caption: `Rewind. <span class="mark-green">Approach 02</span> — generate by shipping a generator.`,
  },
  {
    states: ["scenario-2", "approach-2", "s-prompt", "s-gen-scripted"],
    caption: `Ask for a <em>script</em> that produces data to the given shape.`,
  },
  {
    states: ["scenario-2", "approach-2", "s-prompt", "s-gen-scripted", "s-metadata"],
    caption: `Hand it the schema — the target columns, types, and volume.`,
  },
  {
    states: ["scenario-2", "approach-2", "s-prompt", "s-gen-scripted", "s-metadata", "s-metadata-docked"],
    caption: `Attach the spec.`,
  },
  {
    states: ["scenario-2", "approach-2", "s-prompt", "s-gen-scripted", "s-metadata", "s-metadata-docked", "s-flying-good", "s-cloud-ok"],
    caption: `Over the wire — intent, not content.`,
  },
  {
    states: ["scenario-2", "approach-2", "s-cloud-ok", "s-script-emit"],
    caption: `The model responds with a generator — <span class="mark-ink">prog.py</span>.`,
  },
  {
    states: ["scenario-2", "approach-2", "s-cloud-ok", "s-script-emit", "s-sandbox", "s-ingest-gen"],
    caption: `A disposable sandbox spins up. The script steps inside.`,
  },
  {
    states: ["scenario-2", "approach-2", "s-cloud-ok", "s-sandbox", "s-ingest-gen", "s-gen-csv-sandbox"],
    caption: `Script runs. A fresh dataset materializes — inside the box.`,
  },
  {
    states: ["scenario-2", "approach-2", "s-cloud-ok", "s-sandbox", "s-gen-csv-to-db"],
    caption: `The file lands in the warehouse — no sensitive tokens ever crossed the wire.`,
  },
  {
    states: ["scenario-2", "approach-2", "s-gen-csv-to-db", "s-out-gen"],
    caption: `<span class="mark-green">Data is ready</span> for further processing.`,
  },
];

const stage   = document.getElementById("stage");
const counter = document.getElementById("counter-current");
const total   = document.getElementById("counter-total");
const track   = document.getElementById("track");
const caption = document.getElementById("caption");
const scenNum = document.querySelector(".scen-num");

// deep-link support: #N in the URL jumps to step N (1-based). Clamped to valid range.
function idxFromHash() {
  const n = parseInt(location.hash.replace(/^#/, ""), 10);
  if (!Number.isFinite(n)) return 0;
  return Math.max(0, Math.min(steps.length - 1, n - 1));
}

let idx = idxFromHash();

// build the step track
total.textContent = String(steps.length).padStart(2, "0");
for (let i = 0; i < steps.length; i++) {
  const cell = document.createElement("span");
  cell.className = "track-cell";
  track.appendChild(cell);
}
const cells = Array.from(track.children);

// base classes we always want on the stage (preserve them across step changes)
const baseClasses = ["stage"];
let suppressAnim = true; // first render: skip transitions so the scene draws in its resting state

function apply() {
  const step = steps[idx];
  const cls = [...baseClasses, ...step.states];
  if (suppressAnim) cls.push("no-anim");
  stage.className = cls.join(" ");
  counter.textContent = String(idx + 1).padStart(2, "0");
  scenNum.textContent = step.states.includes("scenario-2") ? "02" : "01";
  cells.forEach((c, i) => {
    c.classList.toggle("on", i < idx);
    c.classList.toggle("current", i === idx);
  });
  caption.classList.remove("show");
  // reflow + delayed show for a soft cross-fade feel
  void caption.offsetWidth;
  caption.innerHTML = step.caption;
  requestAnimationFrame(() => caption.classList.add("show"));

  // sync the URL hash to the current step (replaceState to keep browser history clean)
  const desired = "#" + (idx + 1);
  if (location.hash !== desired) {
    history.replaceState(null, "", desired);
  }
}

function next() { if (idx < steps.length - 1) { idx++; apply(); } }
function prev() { if (idx > 0)                { idx--; apply(); } }
function reset(){ idx = 0; apply(); }

document.addEventListener("keydown", (e) => {
  switch (e.key) {
    case " ":
    case "ArrowRight":
    case "PageDown":
    case "Enter":
      e.preventDefault(); next(); break;
    case "ArrowLeft":
    case "PageUp":
    case "Backspace":
      e.preventDefault(); prev(); break;
    case "Home":
    case "r":
    case "R":
      e.preventDefault(); reset(); break;
    case "End":
      e.preventDefault(); idx = steps.length - 1; apply(); break;
  }
});

// click to advance (but not when clicking hints / track)
document.addEventListener("click", (e) => {
  if (e.target.closest(".chrome-bottom") || e.target.closest(".chrome-top")) return;
  next();
});

// hashchange: react to manual URL edits or external deep links
window.addEventListener("hashchange", () => {
  const target = idxFromHash();
  if (target !== idx) { idx = target; apply(); }
});

apply();
// release the transition guard on the next frame so subsequent step changes animate
requestAnimationFrame(() => {
  requestAnimationFrame(() => {
    suppressAnim = false;
    stage.classList.remove("no-anim");
  });
});
