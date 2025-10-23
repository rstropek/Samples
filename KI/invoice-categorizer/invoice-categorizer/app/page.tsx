export default function Home() {
  return (
    <div className="flex min-h-[calc(100vh-4rem)] items-center justify-center">
      <main className="flex flex-col items-center gap-8 p-8">
        <h1 className="text-4xl font-bold">Welcome to University Tools!</h1>
        <p className="text-gray-600 dark:text-gray-400">
          Select a tool from the menu above to get started.
        </p>
      </main>
    </div>
  );
}
