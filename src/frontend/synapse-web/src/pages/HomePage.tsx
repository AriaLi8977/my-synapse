import { useEffect, useState } from "react";
import { noteHub } from "../signalr/noteHub";
import { createNote, getNotes } from "../api/notesApi";
import type { Note } from "../types/note";
import { NoteCard } from "../components/NoteCard";
import { NoteForm } from "../components/NoteForm";
import { Navbar } from "../components/Navbar";

interface Props{
    onLogout: ()=> void;
}

export function HomePage({onLogout}: Props){
    const [notes, setNotes] = useState<Note[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(()=>{
        async function loadNotes(){ 
            try{
                setLoading(true);
                const data = await getNotes();
                setNotes(data);
            }catch{
                setError("Failed to load notes.");
            }finally{
                setLoading(false);
            }
        }
        loadNotes();

        noteHub.start();

        noteHub.onNoteCompleted((data)=>{
            setNotes((prev)=>
                prev.map((note)=>
                    note.id === data.noteId ? {...note, status: "Completed", summary: data.summary} : note)
            )
        })
    }, []);

    const handleCreate = async (content: string) => {
        const newNote = await createNote(content);
        setNotes((prev) => [...prev, newNote]);
        await noteHub.joinNoteGroup(newNote.id);
    };

    return (
        <div className="min-h-screen bg-gray-100 p-8">
          <div className="max-w-3xl mx-auto">
      
            <Navbar onLogout={onLogout} />
      
            <div className="bg-white rounded-xl shadow p-6">
      
              <h1 className="text-3xl font-bold mb-6">
                Synapse AI Notes
              </h1>
      
              <NoteForm onCreate={handleCreate} />
      
            </div>
      
            {loading && (
              <div className="mt-6 text-gray-600">
                Loading notes...
              </div>
            )}
      
            {error && (
              <div className="mt-6 text-red-500">
                {error}
              </div>
            )}
      
            <div className="mt-6 space-y-4">
              {notes.map((note) => (
                <NoteCard key={note.id} note={note} />
              ))}
            </div>
          </div>
        </div>
      );
};