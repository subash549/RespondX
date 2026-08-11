// ========== Dashboard JavaScript ==========

// ========== Charts ==========
var DashboardCharts = {
    charts: {},

    createBarChart: function (elementId, data, options) {
        var ctx = document.getElementById(elementId);
        if (!ctx) return;

        var defaultOptions = {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: options.legend !== false
                }
            },
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        };

        var chart = new Chart(ctx, {
            type: 'bar',
            data: data,
            options: $.extend(true, defaultOptions, options || {})
        });

        this.charts[elementId] = chart;
        return chart;
    },

    createLineChart: function (elementId, data, options) {
        var ctx = document.getElementById(elementId);
        if (!ctx) return;

        var defaultOptions = {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: options.legend !== false
                }
            },
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        };

        var chart = new Chart(ctx, {
            type: 'line',
            data: data,
            options: $.extend(true, defaultOptions, options || {})
        });

        this.charts[elementId] = chart;
        return chart;
    },

    createPieChart: function (elementId, data, options) {
        var ctx = document.getElementById(elementId);
        if (!ctx) return;

        var defaultOptions = {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'bottom'
                }
            }
        };

        var chart = new Chart(ctx, {
            type: 'pie',
            data: data,
            options: $.extend(true, defaultOptions, options || {})
        });

        this.charts[elementId] = chart;
        return chart;
    },

    createDoughnutChart: function (elementId, data, options) {
        var ctx = document.getElementById(elementId);
        if (!ctx) return;

        var defaultOptions = {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'bottom'
                }
            }
        };

        var chart = new Chart(ctx, {
            type: 'doughnut',
            data: data,
            options: $.extend(true, defaultOptions, options || {})
        });

        this.charts[elementId] = chart;
        return chart;
    },

    destroy: function (elementId) {
        if (this.charts[elementId]) {
            this.charts[elementId].destroy();
            delete this.charts[elementId];
        }
    },

    destroyAll: function () {
        for (var key in this.charts) {
            this.charts[key].destroy();
        }
        this.charts = {};
    }
};

// ========== Data Loading ==========
var DataLoader = {
    loadDashboardStats: function (url, callback) {
        $.ajax({
            url: url || '/Dashboard/GetStats',
            method: 'GET',
            success: function (response) {
                if (response.success) {
                    if (callback) callback(response.data);
                } else {
                    showNotification('Failed to load dashboard stats', 'error');
                }
            },
            error: function () {
                showNotification('Error loading dashboard stats', 'error');
            }
        });
    },

    loadRecentActivity: function (url, callback) {
        $.ajax({
            url: url || '/Dashboard/GetRecentActivity',
            method: 'GET',
            success: function (response) {
                if (response.success) {
                    if (callback) callback(response.data);
                } else {
                    showNotification('Failed to load recent activity', 'error');
                }
            },
            error: function () {
                showNotification('Error loading recent activity', 'error');
            }
        });
    },

    loadNotifications: function (url, callback) {
        $.ajax({
            url: url || '/Dashboard/GetNotifications',
            method: 'GET',
            success: function (response) {
                if (response.success) {
                    if (callback) callback(response.data);
                }
            },
            error: function () {
                // Silent fail for notifications
            }
        });
    }
};

// ========== UI Updates ==========
var DashboardUI = {
    updateStats: function (stats) {
        $('.stat-number').each(function () {
            var key = $(this).data('stat');
            if (stats && stats[key] !== undefined) {
                $(this).text(stats[key]);
                $(this).fadeIn();
            }
        });
    },

    updateActivityFeed: function (activities) {
        var container = $('.activity-list');
        if (!container.length) return;

        container.empty();

        if (activities && activities.length > 0) {
            $.each(activities, function (index, activity) {
                var icon = this.getActivityIcon(activity.type);
                var html = '<li class="activity-item">' +
                    '<div class="activity-icon ' + icon.class + '">' +
                    '<i class="fas fa-' + icon.icon + '"></i>' +
                    '</div>' +
                    '<div class="activity-content">' +
                    '<h4>' + activity.title + '</h4>' +
                    '<p>' + activity.description + '</p>' +
                    '</div>' +
                    '<span class="activity-time">' + timeAgo(activity.timestamp) + '</span>' +
                    '</li>';
                container.append(html);
            });
        } else {
            container.append('<li class="text-center text-muted">No recent activity</li>');
        }
    },

    getActivityIcon: function (type) {
        var icons = {
            'completed': { icon: 'check-circle', class: 'completed' },
            'started': { icon: 'play-circle', class: 'started' },
            'alert': { icon: 'exclamation-circle', class: 'alert' },
            'quiz': { icon: 'question-circle', class: 'started' },
            'certificate': { icon: 'award', class: 'completed' },
            'scenario': { icon: 'users', class: 'started' }
        };
        return icons[type] || { icon: 'circle', class: 'started' };
    },

    updateNotificationBadge: function (count) {
        var badge = $('.notification-badge');
        if (count > 0) {
            badge.text(count).show();
        } else {
            badge.hide();
        }
    },

    toggleSidebar: function () {
        $('.sidebar').toggleClass('active');
        $('.main-content').toggleClass('expanded');
    }
};

// ========== Real-time Updates ==========
var RealTimeMonitor = {
    intervalId: null,
    lastUpdate: null,

    startMonitoring: function (interval) {
        var self = this;
        interval = interval || 30000; // Default: 30 seconds

        if (this.intervalId) {
            clearInterval(this.intervalId);
        }

        this.intervalId = setInterval(function () {
            self.checkUpdates();
        }, interval);

        // Initial check
        this.checkUpdates();
    },

    stopMonitoring: function () {
        if (this.intervalId) {
            clearInterval(this.intervalId);
            this.intervalId = null;
        }
    },

    checkUpdates: function () {
        $.ajax({
            url: '/Dashboard/CheckUpdates',
            method: 'GET',
            data: { lastUpdate: this.lastUpdate },
            success: function (response) {
                if (response.success && response.hasUpdates) {
                    RealTimeMonitor.lastUpdate = response.timestamp;

                    if (response.notifications) {
                        DashboardUI.updateNotificationBadge(response.notificationCount);
                    }

                    if (response.stats) {
                        DashboardUI.updateStats(response.stats);
                    }

                    if (response.activities) {
                        DashboardUI.updateActivityFeed(response.activities);
                    }

                    if (response.showNotification) {
                        showNotification(response.message, 'info');
                    }
                }
            }
        });
    }
};

// ========== Export Functions ==========
var Dashboard = {
    charts: DashboardCharts,
    data: DataLoader,
    ui: DashboardUI,
    realTime: RealTimeMonitor
};

// ========== Dashboard Document Ready ==========
$(document).ready(function () {
    // Initialize charts if Chart.js is available
    if (typeof Chart !== 'undefined') {
        // Bar chart example
        if ($('#stats-chart').length) {
            Dashboard.charts.createBarChart('stats-chart', {
                labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
                datasets: [{
                    label: 'Completed Modules',
                    data: [12, 19, 15, 22, 28, 34],
                    backgroundColor: 'rgba(102, 126, 234, 0.5)',
                    borderColor: 'rgba(102, 126, 234, 1)',
                    borderWidth: 1
                }]
            });
        }

        // Line chart example
        if ($('#progress-chart').length) {
            Dashboard.charts.createLineChart('progress-chart', {
                labels: ['Week 1', 'Week 2', 'Week 3', 'Week 4'],
                datasets: [{
                    label: 'Learning Progress',
                    data: [20, 45, 60, 75],
                    backgroundColor: 'rgba(102, 126, 234, 0.1)',
                    borderColor: 'rgba(102, 126, 234, 1)',
                    borderWidth: 2,
                    fill: true
                }]
            });
        }

        // Pie chart example
        if ($('#distribution-chart').length) {
            Dashboard.charts.createPieChart('distribution-chart', {
                labels: ['Completed', 'In Progress', 'Not Started'],
                datasets: [{
                    data: [45, 35, 20],
                    backgroundColor: [
                        'rgba(72, 187, 120, 0.8)',
                        'rgba(246, 173, 85, 0.8)',
                        'rgba(160, 174, 192, 0.8)'
                    ]
                }]
            });
        }
    }

    // Load dashboard data
    if ($('.dashboard-stats').length) {
        Dashboard.data.loadDashboardStats(null, function (stats) {
            Dashboard.ui.updateStats(stats);
        });

        Dashboard.data.loadRecentActivity(null, function (activities) {
            Dashboard.ui.updateActivityFeed(activities);
        });

        Dashboard.data.loadNotifications(null, function (data) {
            if (data && data.count !== undefined) {
                Dashboard.ui.updateNotificationBadge(data.count);
            }
        });
    }

    // Start real-time monitoring
    if ($('.dashboard-realtime').length) {
        Dashboard.realTime.startMonitoring(30000);
    }

    // Handle sidebar toggle
    $('.sidebar-toggle').click(function () {
        Dashboard.ui.toggleSidebar();
    });

    // Handle quick actions
    $('.quick-action-btn').click(function () {
        var action = $(this).data('action');
        var url = $(this).data('url');

        if (url) {
            window.location.href = url;
        } else if (action) {
            switch (action) {
                case 'new-module':
                    $('#modal-new-module').modal('show');
                    break;
                case 'new-quiz':
                    $('#modal-new-quiz').modal('show');
                    break;
                case 'view-certificates':
                    window.location.href = '/Learner/Certificates.aspx';
                    break;
                default:
                    showNotification('Action: ' + action, 'info');
            }
        }
    });
});

// ========== Cleanup ==========
$(window).on('beforeunload', function () {
    Dashboard.realTime.stopMonitoring();
    Dashboard.charts.destroyAll();
});