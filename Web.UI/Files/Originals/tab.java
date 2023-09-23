package {{packageName}}.tabs;

import android.Manifest;
import android.app.AlertDialog;
import android.content.ContentValues;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.media.RingtoneManager;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.os.Environment;
import android.provider.MediaStore;
import android.provider.Settings;
import android.support.annotation.Nullable;
import android.support.design.widget.Snackbar;
import android.support.v4.app.ActivityCompat;
import android.support.v4.app.Fragment;
import android.support.v4.content.ContextCompat;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.BaseAdapter;
import android.widget.Button;
import android.widget.GridView;
import android.widget.Toast;

import {{packageName}}.MainActivity;
import {{packageName}}.R;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;

public class Tab{{tabNumber}} extends Fragment {
    GridView myGridView;
    int position;
    View layout;
    File directory;

    public String[] items = {
            {{soundStrings}}
    };

    public static int[] soundfiles = {
            {{soundPaths}}
    };

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View rootView = inflater.inflate(R.layout.tab{{tabNumber}}_layout, container, false);
        layout = rootView.findViewById(R.id.tab{{tabNumber}});
        File storage = Environment.getExternalStorageDirectory();
        directory = new File(storage.getAbsolutePath() + "/" + R.string.foldername + "/");

        // GridView
        myGridView = (GridView) rootView.findViewById(R.id.tab{{tabNumber}}GridView);
        myGridView.setAdapter(new CustomGridAdapter(getActivity(), items));
        myGridView.setOnItemLongClickListener(new AdapterView.OnItemLongClickListener() {
            @Override
            public boolean onItemLongClick(AdapterView<?> arg0, View arg1,
                                           final int pos, long id) {
                position = pos;
                AlertDialog.Builder builder = new AlertDialog.Builder(getContext(), android.R.style.Theme_Material_Light_Dialog_Alert);
                builder.setItems(new CharSequence[]{getText(R.string.share_sound_title), getText(R.string.set_tone_as_title)}, new DialogInterface.OnClickListener() {

                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        switch (which) {
                            case 0:
                                if (ContextCompat.checkSelfPermission(getContext(), Manifest.permission.WRITE_EXTERNAL_STORAGE) != PackageManager.PERMISSION_GRANTED) {
                                    ActivityCompat.requestPermissions(getActivity(), new String[]{Manifest.permission.WRITE_EXTERNAL_STORAGE}, 0);
                                } else {
                                    savefile(pos, true);
                                    Intent share = new Intent(Intent.ACTION_SEND);
                                    share.putExtra(Intent.EXTRA_STREAM, Uri.parse(Environment.getExternalStorageDirectory().toString() + "/" + R.string.foldername + "/" + items[position] + ".mp3"));
                                    share.setType("audio/mp3");
                                    startActivity(Intent.createChooser(share, getText(R.string.share_sound_via)));
                                }
                                break;
                            case 1:
                                requestPermissions();
                                if (ContextCompat.checkSelfPermission(getContext(), Manifest.permission.WRITE_EXTERNAL_STORAGE) == PackageManager.PERMISSION_GRANTED) {
                                    if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
                                        if (Settings.System.canWrite(getContext())) {
                                            buildalertdielog_withpermissions();
                                            savefile(pos, false);
                                        }
                                    } else {
                                        buildalertdielog_withpermissions();
                                        savefile(pos, false);
                                    }
                                }
                                break;
                        }
                    }
                });
                builder.create();
                builder.show();
                return true;
            }
        });
        return rootView;
    }

    // CustomGrid Adapter
    public class CustomGridAdapter extends BaseAdapter {
        private Context context;
        private String[] items;
        LayoutInflater inflater;

        public CustomGridAdapter(Context c, String[] items) {
            this.context = c;
            this.items = items;
            inflater = (LayoutInflater) this.context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
        }

        @Override
        public int getCount() {
            return items.length;
        }

        @Override
        public Object getItem(int position) {
            return items[position];
        }

        @Override
        public long getItemId(int position) {
            return position;
        }

        @Override
        public View getView(final int position, View convertView, ViewGroup parent) {

            if (convertView == null) {
                convertView = inflater.inflate(R.layout.single_item, null);
            }
            Button button = (Button) convertView.findViewById(R.id.button);
            button.setText(items[position]);
            button.setOnClickListener(new View.OnClickListener() {

                @Override
                public void onClick(View v) {
                    if (context instanceof MainActivity) {
                        ((MainActivity) context).Tab{{tabNumber}}ItemClicked(position);
                    }
                }
            });

            return convertView;
        }
    }

    // check if the permission to write external storage for sharing and setting sounds isn't already granted, if not -> shows a snackbar to get the permission (neccessary for setting sounds as ringtone etc.)
    private void requestPermissions() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
            if (ContextCompat.checkSelfPermission(getContext(), Manifest.permission.WRITE_EXTERNAL_STORAGE) != PackageManager.PERMISSION_GRANTED) {
                ActivityCompat.requestPermissions(getActivity(), new String[]{Manifest.permission.WRITE_EXTERNAL_STORAGE}, 0);
            }
            if (!Settings.System.canWrite(getContext())) {
                Snackbar.make(layout, getText(R.string.notice_that_app_needs_access_to_settings), Snackbar.LENGTH_INDEFINITE).setAction("OK",
                        new View.OnClickListener() {
                            @Override
                            public void onClick(View v) {
                                Context context = v.getContext();
                                Intent intent = new Intent(Settings.ACTION_MANAGE_WRITE_SETTINGS);
                                intent.setData(Uri.parse("package:" + context.getPackageName()));
                                intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                                startActivity(intent);
                            }
                        }).show();
            }
        }
    }

    // Builds dialog for setting ringtone etc.
    public void buildalertdielog_withpermissions() {
        AlertDialog.Builder builder;
        if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.LOLLIPOP) {
            builder = new AlertDialog.Builder(getContext(), android.R.style.Theme_Material_Light_Dialog_Alert);
        } else {
            builder = new AlertDialog.Builder(getContext(), AlertDialog.THEME_HOLO_LIGHT);
        }

        builder.setItems(new CharSequence[]{getText(R.string.ringtone_title), getText(R.string.notification_title), getText(R.string.alarm_title)}, new DialogInterface.OnClickListener() {

            @Override
            public void onClick(DialogInterface dialog, int pos) {
                switch (pos) {

                    // Ringtone
                    case 0:
                        Toast.makeText(getContext(), getText(R.string.ringtone_title), Toast.LENGTH_SHORT).show();
                        setTone(1);
                        break;
                    // Notification
                    case 1:
                        Toast.makeText(getContext(), getText(R.string.notification_title), Toast.LENGTH_SHORT).show();
                        setTone(2);
                        break;
                    // Alarmton
                    case 2:
                        Toast.makeText(getContext(), getText(R.string.alarm_title), Toast.LENGTH_SHORT).show();
                        setTone(3);
                        break;
                }
            }
        });
        builder.create();
        builder.show();
    }

    // Saves sounds for sharing or saving as ringtone etc.
    public void savefile(int pos, boolean sharing) {
        File file;
        File storage = Environment.getExternalStorageDirectory();
        File directory = new File(storage.getAbsolutePath() + "/" + R.string.foldername + "/");
        directory.mkdirs();

        if (sharing) {
            file = new File(directory, items[position] + ".mp3");
        } else {
            file = new File(directory, items[position]);
        }

        InputStream in = this.getResources().openRawResource(soundfiles[pos]);
        try {
            OutputStream out = new FileOutputStream(file);
            byte[] buffer = new byte[1024];
            int len;
            while ((len = in.read(buffer, 0, buffer.length)) != -1) {
                out.write(buffer, 0, len);
            }

            in.close();
            out.close();
        } catch (IOException e) {
            Log.e("Failed to save file: ", "###");
        }
    }

    // Sets sounds as Ringtone, Notification or Alarm
    public void setTone(int action) {
        File soundfile = new File(directory, items[position]);
        try {

            ContentValues values = new ContentValues();
            values.put(MediaStore.MediaColumns.DATA, soundfile.getAbsolutePath());
            values.put(MediaStore.MediaColumns.TITLE, items[position]);
            values.put(MediaStore.MediaColumns.MIME_TYPE, "audio/*");

            switch (action) {

                // Ringtone
                case 1:
                    values.put(MediaStore.Audio.Media.IS_RINGTONE, true);
                    values.put(MediaStore.Audio.Media.IS_NOTIFICATION, false);
                    values.put(MediaStore.Audio.Media.IS_ALARM, false);
                    break;

                // Notification
                case 2:
                    values.put(MediaStore.Audio.Media.IS_RINGTONE, false);
                    values.put(MediaStore.Audio.Media.IS_NOTIFICATION, true);
                    values.put(MediaStore.Audio.Media.IS_ALARM, false);
                    break;

                // Alarm
                case 3:
                    values.put(MediaStore.Audio.Media.IS_RINGTONE, false);
                    values.put(MediaStore.Audio.Media.IS_NOTIFICATION, false);
                    values.put(MediaStore.Audio.Media.IS_ALARM, true);
                    break;
            }

            values.put(MediaStore.Audio.Media.IS_MUSIC, false);
            Uri uri = MediaStore.Audio.Media.getContentUriForPath(soundfile.getAbsolutePath());
            getContext().getContentResolver().delete(uri, MediaStore.MediaColumns.DATA + "=\"" + soundfile.getAbsolutePath() + "\"", null);
            Uri finalUri = getContext().getContentResolver().insert(uri, values);

            switch (action) {

                // Ringtone
                case 1:
                    RingtoneManager.setActualDefaultRingtoneUri(getContext(), RingtoneManager.TYPE_RINGTONE, finalUri);
                    break;
                // Notification
                case 2:
                    RingtoneManager.setActualDefaultRingtoneUri(getContext(), RingtoneManager.TYPE_NOTIFICATION, finalUri);
                    break;
                // Alarm
                case 3:
                    RingtoneManager.setActualDefaultRingtoneUri(getContext(), RingtoneManager.TYPE_ALARM, finalUri);
                    break;
            }

        } catch (Exception e) {
            Log.e("Failed to save: ", "######");
        }
    }
}

