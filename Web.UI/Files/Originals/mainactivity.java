package {{packageName}};

import android.content.Intent;
import android.media.MediaPlayer;
import android.os.Bundle;
import android.os.Environment;
import android.support.design.widget.NavigationView;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.support.v4.widget.DrawerLayout;
import android.support.v7.app.ActionBarDrawerToggle;
import android.support.v7.app.AppCompatActivity;
import android.util.Log;
import android.view.MenuItem;
import android.view.View;
import android.widget.RelativeLayout;
import android.widget.Toast;

{{tabImports}}
import com.unity3d.ads.IUnityAdsListener;
import com.unity3d.ads.UnityAds;
import com.unity3d.services.banners.BannerView;
import com.unity3d.services.banners.IUnityBannerListener;
import com.unity3d.services.banners.UnityBannerSize;

import java.io.File;

import hotchemi.android.rate.AppRate;

public class MainActivity extends AppCompatActivity implements IUnityAdsListener, IUnityBannerListener {
    public MediaPlayer mp;
    DrawerLayout mDrawerLayout;
    NavigationView mNavigationView;
    FragmentManager mFragmentManager;
    FragmentTransaction mFragmentTransaction;
    int sound_played_counter;

    BannerView bottomBanner;
    RelativeLayout bottomBannerView;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        // Initialize the Unity SDK:
        UnityAds.initialize(this, getString(R.string.unity_game_id), getResources().getBoolean(R.bool.unity_test_mode), getResources().getBoolean(R.bool.unity_enable_load));

        // Banner
        // Create the top banner view object:
        bottomBanner = new BannerView(this, getString(R.string.unity_banner_placement), new UnityBannerSize(320, 50));
        // Set the listener for banner lifcycle events:
        // bottomBanner.setListener(bannerListener);

        // Request a banner ad:
        bottomBanner.load();

        // Get the banner view:
        bottomBannerView = findViewById(R.id.bottomBanner);
        bottomBannerView.addView(bottomBanner);

        // Rating Dialog
        AppRate.with(this)
                .setInstallDays(0)
                .setLaunchTimes(3)
                .setRemindInterval(2)
                .monitor();

        // Show a dialog if meets conditions
        AppRate.showRateDialogIfMeetsConditions(this);

        toolbarTabs();
        sidebar();
        externalStorageAccess();

    }

    // Creates sidebar and sets onClickListeners
    public void sidebar() {
        mNavigationView = (NavigationView) findViewById(R.id.navigationView);
        mNavigationView.setItemIconTintList(null);
        mNavigationView.setNavigationItemSelectedListener(new NavigationView.OnNavigationItemSelectedListener() {
            @Override
            public boolean onNavigationItemSelected(MenuItem menuItem) {
                mDrawerLayout.closeDrawers();

                switch (menuItem.getItemId()) {
                    case R.id.sounds:
                        FragmentTransaction xfragmentTransaction = mFragmentManager.beginTransaction();
                        xfragmentTransaction.replace(R.id.containerView, new TabFragment()).commit();
                        break;

                    case R.id.share:
                        Intent shareIntent = new Intent(Intent.ACTION_SEND);
                        shareIntent.setType("text/plain");
                        shareIntent.putExtra(Intent.EXTRA_SUBJECT, getText(R.string.app_name));
                        shareIntent.putExtra(Intent.EXTRA_TEXT, getText(R.string.share_text) + " " + getText(R.string.app_name) + "\n\n" + getText(R.string.playstore_link));
                        startActivity(Intent.createChooser(shareIntent, getText(R.string.share_via)));
                        break;

                }
                return false;
            }

        });
    }

    {{tabClickMethods}}

    // Cleans MediaPlayer
    public void cleanUpMediaPlayer() {
        if (mp != null) {
            try {
                mp.stop();
                mp.release();
                mp = null;
            } catch (Exception e) {
                e.printStackTrace();
                Toast.makeText(MainActivity.this, "Error", Toast.LENGTH_LONG).show();

            }
        }
    }

    // Access external storage
    public void externalStorageAccess() {
        final File FILES_PATH = new File(Environment.getExternalStorageDirectory(), "Android/data/" + getText(R.string.package_name) + "/files");
        if (Environment.MEDIA_MOUNTED.equals(
                Environment.getExternalStorageState())) {
            if (!FILES_PATH.mkdirs()) {
                Log.w("error", "Could not create " + FILES_PATH);
            }
        } else {
            Toast.makeText(MainActivity.this, "Error", Toast.LENGTH_LONG).show();
            finish();
        }
    }

    // Creates toolbar Tabs
    public void toolbarTabs() {
        mFragmentManager = getSupportFragmentManager();
        mFragmentTransaction = mFragmentManager.beginTransaction();
        mFragmentTransaction.replace(R.id.containerView, new TabFragment()).commit();
        mDrawerLayout = (DrawerLayout) findViewById(R.id.drawerLayout);
        android.support.v7.widget.Toolbar toolbar = (android.support.v7.widget.Toolbar) findViewById(R.id.toolbar);
        ActionBarDrawerToggle mDrawerToggle = new ActionBarDrawerToggle(this, mDrawerLayout, toolbar, R.string.app_name,
                R.string.app_name);
        mDrawerLayout.setDrawerListener(mDrawerToggle);
        mDrawerToggle.syncState();
    }

    // Implement a function to display an ad if the Placement is ready:
    public void DisplayInterstitialAd() {
        if (UnityAds.isReady(getString(R.string.unity_interstitial_placement))) {
            UnityAds.show(this);
        }
    }

    @Override
    public void onUnityAdsReady(String s) {

    }

    @Override
    public void onUnityAdsStart(String s) {

    }

    @Override
    public void onUnityAdsFinish(String s, UnityAds.FinishState finishState) {

    }

    @Override
    public void onUnityAdsError(UnityAds.UnityAdsError unityAdsError, String s) {

    }

    @Override
    public void onUnityBannerLoaded(String s, View view) {

    }

    @Override
    public void onUnityBannerUnloaded(String s) {

    }

    @Override
    public void onUnityBannerShow(String s) {

    }

    @Override
    public void onUnityBannerClick(String s) {

    }

    @Override
    public void onUnityBannerHide(String s) {

    }

    @Override
    public void onUnityBannerError(String s) {

    }
}
