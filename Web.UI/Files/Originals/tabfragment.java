package {{packageName}};

import android.os.Bundle;
import android.support.annotation.Nullable;
import android.support.design.widget.TabLayout;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentPagerAdapter;
import android.support.v4.view.ViewPager;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

{{tabImports}}
import com.unity3d.ads.IUnityAdsListener;
import com.unity3d.ads.UnityAds;

public class TabFragment extends Fragment implements IUnityAdsListener {
    public static TabLayout tabLayout;
    public static ViewPager viewPager;
    int tab_change_counter;

    public static int int_items = {{tabAmount}};

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        // Initialize the Unity SDK:
        UnityAds.initialize(this.getActivity(), getString(R.string.unity_game_id), getResources().getBoolean(R.bool.unity_test_mode), getResources().getBoolean(R.bool.unity_enable_load));

        View v = inflater.inflate(R.layout.tab_layout, null);
        tabLayout = (TabLayout) v.findViewById(R.id.tabs);
        viewPager = (ViewPager) v.findViewById(R.id.viewpager);
        viewPager.setAdapter(new MyAdapter(getChildFragmentManager()));

        // Manage Interstitial Ads
        viewPager.setOnPageChangeListener(new ViewPager.OnPageChangeListener() {
            public void onPageScrollStateChanged(int state) {
            }

            public void onPageScrolled(int position, float positionOffset, int positionOffsetPixels) {
            }

            public void onPageSelected(int position) {
                tab_change_counter++;

                if (Integer.parseInt(getText(R.string.interstitial_ad_frequency_tab).toString()) == tab_change_counter) {
                    DisplayInterstitialAd();
                    tab_change_counter = 0;
                }
            }
        });
        tabLayout.post(new Runnable() {
            @Override
            public void run() {
                tabLayout.setupWithViewPager(viewPager);
            }
        });
        return v;
    }

    class MyAdapter extends FragmentPagerAdapter {
        public MyAdapter(FragmentManager fm) {
            super(fm);
        }

        // Tab positions
        @Override
        public Fragment getItem(int position) {
            {{tabGetItem}}
            return null;
        }

        // Tab titles
        @Override
        public CharSequence getPageTitle(int position) {
            switch (position) {
                {{tabPositions}}
            }
            return null;
        }

        @Override
        public int getCount() {
            return int_items;
        }
    }

    public void DisplayInterstitialAd() {
        if (UnityAds.isReady(getString(R.string.unity_interstitial_placement))) {
            UnityAds.show(this.getActivity());
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
}
