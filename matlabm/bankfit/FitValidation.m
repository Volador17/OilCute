function [allTQ,allSQ,allRatio,allIdx,allFit] = FitValidation(x,vx,wind,waveIdx)
% �������Ϸ�������֤
% ���룺x������ף�pr����������ʣ�wind���ж�����Ƿ��ֽ����ƶ����ϵ����ʶ���������Ĵ��ڿ�ȣ�%vx����֤�����ף�vy����֤������
%wave1,�ж�����Ƿ��ֽ����ƶ����ϵ����ʶ���������Ĺ��������ʼλ�ã�wave2���ж�����Ƿ��ֽ����ƶ����ϵ����ʶ���������Ĺ�����������λ�ã�
% �����result��������ʾ��minSQ����С�ƶ����ϵ��,TQ��ʶ�������
% result����300+1��*���ʣ���1��Ϊ��ʶ����Ʒ�����ʣ���2�������������У�Ϊʶ������Ŀ���Ʒ�����ʣ�������������
% result2��ÿһ�дӵ�һ�е����������У�Ϊ������ϵĿ���Ʒ�ı���
% minSQ��1*У������Ʒ��Ŀ
% TQ��1*У������Ʒ��Ŀ
	xcount = size(x,2);
    [m,n] = size(vx);
	allTQ = zeros(n,1);
	allSQ = zeros(n,1);
	allFit = zeros(m,n);
    allRatio = zeros(xcount,n);
    allIdx = zeros(xcount,n);
    maxlen = 0;
	for ii=1:n
		y = vx(:,ii);
		[TQ, SQ, ratio, idx, fit ] = FitPredictor(x,y,wind,waveIdx,1);
		allTQ(ii) = TQ;
		allSQ(ii) = SQ; 
		allRatio(1:length(ratio),ii) = ratio;
		allIdx(1:length(ratio),ii) = idx;
		allFit(:,ii) = fit;
        if maxlen<length(ratio)
            maxlen = length(ratio);    
        end
	end
    if maxlen>0
        allRatio = allRatio(1:maxlen,:);
        allIdx = allIdx(1:maxlen,:);
    end
end